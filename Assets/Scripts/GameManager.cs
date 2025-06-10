using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static Action onCardDisplay;
    public RectTransform player; // Player UI image
    public RectTransform[] waypoints; // All 38 waypoints as UI elements
    public int currentWaypointIndex = 0;
    public int remainingMoves = 10;
    public float moveSpeed = 1000f;
    public GameObject MoveFinished;
    public List<CardDisplay> cards = new List<CardDisplay>();   
    public CardHolder cardHolder;

    public Inventory inventory; // Reference to inventory system

    private List<string> objectname= new List<string>();    

    public void MovePlayer(int steps)
    {
        if (remainingMoves <= 0 || isMoving) return;

        remainingMoves--;
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
        if (inventory != null)
        {
            string itemName = waypoints[currentWaypointIndex].GetComponentInChildren<CardDisplay>().Card.name;
            Debug.Log("Step finished at waypoint: " + itemName);

            inventory.AddItem(itemName);
            inventory.DisplayInventory();     // show the list

        }

        isMoving = false;
    }

    private void Start()
    {
        if (waypoints.Length > 0 && player != null)
        {
            currentWaypointIndex = 0;
            player.anchoredPosition = waypoints[0].anchoredPosition;
        }
        SetCards();
        onCardDisplay?.Invoke();
    }
    private void SetCards()
    {
        foreach (var card in cards)
        {
            var rand = UnityEngine.Random.Range(0, cardHolder.CardHold.Count);
            card.Card = cardHolder.CardHold[rand];
        }
    }
}
