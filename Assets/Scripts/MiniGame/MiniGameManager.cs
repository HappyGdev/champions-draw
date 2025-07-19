using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager instance;
    public static System.Action onCardDisplay;

    [Header("Player Section")]
    public RectTransform player;
    public TextMeshProUGUI timerText;
    [HideInInspector] public float moveSpeed = 1000f;
    [HideInInspector] public bool isMoving = false;

    [Header("Timer")]
    public float startTime = 10f;
    public float currentTime;
    private bool gameActive = false;

    [Header("Boss Stats")]
    public int bossHP = 100;
    public TextMeshProUGUI bossHpText;

    [Header("Waypoints")]
    public WaypointManager waypoints;
    private int currentWaypointIndex = 0;

    [Header("Cards")]
    public CardHolder cardHolder;
    public Card NullCard;
    public int Null_Card_Amounts;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // init
        currentTime = startTime;
        gameActive = true;
        UpdateTimerUI();

        // Set start position
        if (waypoints.Wayp.Length > 0 && player != null)
        {
            currentWaypointIndex = 0;
            player.anchoredPosition = waypoints.Wayp[0].GetComponent<RectTransform>().anchoredPosition;
        }

        SetCards();
        SetNullCards();

        onCardDisplay?.Invoke();

        StartCoroutine(TimerRoutine());
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
    }

    IEnumerator TimerRoutine()
    {
        while (currentTime > 0f && gameActive)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (gameActive)
        {
            GameOver(false); // time ended
        }
    }

    public void AddTime(int seconds)
    {
        currentTime += seconds;
        UpdateTimerUI();
    }

    public void MovePlayer(int steps)
    {
        if (!gameActive || isMoving) return;

        AddTime(steps); // Add dice number to timer
        StartCoroutine(MoveOverSteps(steps));
    }

    IEnumerator MoveOverSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Wayp.Length;

            Vector2 targetPos = waypoints.Wayp[currentWaypointIndex].GetComponent<RectTransform>().anchoredPosition;
            while (Vector2.Distance(player.anchoredPosition, targetPos) > 1f)
            {
                player.anchoredPosition = Vector2.MoveTowards(
                    player.anchoredPosition,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        // After move, check if card exists
        CardDisplay cardDisplay = waypoints.Wayp[currentWaypointIndex].GetComponentInChildren<CardDisplay>();
        if (cardDisplay != null && cardDisplay.Card != null && cardDisplay.Card.actionType != CardActionType.empty)
        {
            bossHP -= cardDisplay.Card.value1;
            bossHpText.text = "Boss HP: " + bossHP.ToString();

            if (bossHP <= 0)
            {
                GameOver(true); // boss defeated
            }
        }

        isMoving = false;
    }

    private void SetCards()
    {
        List<Card> shuffled = new List<Card>(cardHolder.CardHold);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        for (int i = 0; i < waypoints.cards.Count && i < shuffled.Count; i++)
        {
            waypoints.cards[i].Card = shuffled[i];
        }
    }

    private void SetNullCards()
    {
        HashSet<int> indices = new HashSet<int>();
        while (indices.Count < Null_Card_Amounts)
        {
            int rand = Random.Range(0, waypoints.cards.Count);
            indices.Add(rand);
        }

        foreach (int index in indices)
        {
            if (index >= 0 && index < waypoints.cards.Count && waypoints.cards[index] != null)
            {
                waypoints.cards[index].Card = NullCard;
            }
        }
    }

    void GameOver(bool playerWon)
    {
        gameActive = false;
        StopAllCoroutines();

        if (playerWon)
        {
            timerText.text = "🎉 You Win!";
        }
        else
        {
            timerText.text = "⏰ Time's Up!";
        }
    }
}
