using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static Action onCardDisplay;

    [Header("Player Section")]
    public RectTransform player; // Player UI image
    [HideInInspector]public int remainingMoves = 10;
    public TextMeshProUGUI remainingMove_txt;
    [HideInInspector] public float moveSpeed = 1000f;
    [Space]
    [Header("Waypoints")]
    public RectTransform[] waypoints; // All 38 waypoints as UI elements
    private int currentWaypointIndex = 0;

    public GameObject MoveFinished;
    [Space]
    [Header("cards")]
    public List<CardDisplay> cards = new List<CardDisplay>();   
    public CardHolder cardHolder;

    [Header("Null Card ")]
    public Sprite NullSprite; // Assign a default sprite here
    public Card NullCard;
    public int Null_Card_Amounts;

    [Space]
    [Header("Inventory")]
    public UiItemSpawner uiItemSpawner; // Reference to inventory system
    private List<string> objectname= new List<string>();

    private void Start()
    {
        if (waypoints.Length > 0 && player != null)
        {
            currentWaypointIndex = 0;
            player.anchoredPosition = waypoints[0].anchoredPosition;
        }
        SetCards();
        SetNullCards();
        GiveInitialInventory(); 
        onCardDisplay?.Invoke();
    }

    public void MovePlayer(int steps)
    {
        if (remainingMoves <= 0 || isMoving) return;

        remainingMoves--;
        remainingMove_txt.text = remainingMoves.ToString();
        StartCoroutine(MoveOverSteps(steps));
    }

    private bool isMoving = false;

    IEnumerator MoveOverSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            Vector2 targetPos = waypoints[currentWaypointIndex].anchoredPosition;

            while (Vector2.Distance(player.anchoredPosition, targetPos) > 1f)
            {
                player.anchoredPosition = Vector2.MoveTowards(
                    player.anchoredPosition,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        if (remainingMoves == 0)
        {
            MoveFinished.SetActive(true);
        }
        // Add an item to inventory AFTER completing all steps
        if (uiItemSpawner != null)
        {
            // string itemName = waypoints[currentWaypointIndex].GetComponentInChildren<CardDisplay>().Card.name;
            Card ChosenCard = waypoints[currentWaypointIndex].GetComponentInChildren<CardDisplay>().Card;

            //Debug.Log("Step finished at waypoint: " + itemName);

            if(ChosenCard.actionType != CardActionType.empty)
            {
                uiItemSpawner.SpawnItem(ChosenCard);
                onCardDisplay?.Invoke();
            }
            //inventory.DisplayInventory();     // show the list

        }

        isMoving = false;
    }

    private void SetCards()
    {
        // Clone the original list so the original CardHold remains unchanged
        List<Card> shuffledCards = new List<Card>(cardHolder.CardHold);

        // Shuffle the list
        for (int i = 0; i < shuffledCards.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, shuffledCards.Count);
            // Swap
            Card temp = shuffledCards[i];
            shuffledCards[i] = shuffledCards[randIndex];
            shuffledCards[randIndex] = temp;
        }

        // Assign shuffled cards without repetition
        for (int i = 0; i < cards.Count && i < shuffledCards.Count; i++)
        {
            cards[i].Card = shuffledCards[i];
        }
    }
    private void SetNullCards()
    {
        HashSet<int> uniqueRandomIndices = new HashSet<int>();

        while (uniqueRandomIndices.Count < Null_Card_Amounts)
        {
            int rand = UnityEngine.Random.Range(0, cards.Count); 
            uniqueRandomIndices.Add(rand);
        }

        Debug.Log("Generated Unique Random Numbers: " + string.Join(", ", uniqueRandomIndices));

        foreach (int index in uniqueRandomIndices)
        {
            if (index >= 0 && index < cards.Count && cards[index] != null)
            {
                cards[index].Card = NullCard;
            }
        }
    }
    private void GiveInitialInventory()
    {
        if (cardHolder == null || uiItemSpawner == null || cardHolder.CardHold.Count < 4)
        {
            Debug.LogWarning("CardHolder or UIItemSpawner not assigned properly.");
            return;
        }

        HashSet<int> uniqueIndexes = new HashSet<int>();

        while (uniqueIndexes.Count < 4)
        {
            int randIndex = UnityEngine.Random.Range(0, cardHolder.CardHold.Count);
            uniqueIndexes.Add(randIndex);
        }

        foreach (int index in uniqueIndexes)
        {
            Card randomCard = cardHolder.CardHold[index];
            if (randomCard.actionType != CardActionType.empty)
            {
                uiItemSpawner.SpawnItem(randomCard);
            }
        }
    }
}
