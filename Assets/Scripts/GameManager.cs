using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Action onCardDisplay;

    [Header("Player Section")]
    public RectTransform player; // Player UI image
    public int remainingMoves = 10;
    public TextMeshProUGUI remainingMove_txt;
    [HideInInspector] public float moveSpeed = 1000f;
    [HideInInspector] public bool isMoving = false;
    [Space]

    [Header("Waypoints")]
    public WaypointManager waypoints; // All 38 waypoints as UI elements
    private int currentWaypointIndex = 0;
    [Space]

    [Header("cards")]
    //public List<CardDisplay> cards = new List<CardDisplay>();   
    public CardHolder cardHolder;
    [Space]

    [Header("Null Card ")]
    public Sprite NullSprite; // Assign a default sprite here
    public Card NullCard;
    public int Null_Card_Amounts;
    [Space]

    [Header("Inventory")]
    public UiItemSpawner uiItemSpawner; // Reference to inventory system
    public List<Card> PlayerFightcards = new List<Card>();
    [Space]

    [Header("Boss Section")]
    public GameObject MainPanel;
    public GameObject BossPanel;
    public List<CardDisplay> BossCard = new List<CardDisplay>();
    [Space]

    [Header("Combat Stats")]
    public int playerHP = 100;
    public int bossHP = 200;

    private bool playerTurn = true;
    [HideInInspector]public bool gameOver = false;


    [SerializeField] private int bossPoisendCount;
    private bool isPlayerProtected;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    private void Start()
    {
        //set panels
        MainPanel.SetActive(true);
        BossPanel.SetActive(false);

        // Set Initial Player Place to Start Game (Waypoint zero)
        if (waypoints.Wayp.Length > 0 && player != null)
        {
            currentWaypointIndex = 0;
            player.anchoredPosition = waypoints.Wayp[0].GetComponent<RectTransform>().anchoredPosition;
        }

        //Set Shuffle Card From CardHolder in Player Card Section
        SetCards();

        //Set given Amount of Empty Card on Board
        SetNullCards();

        //Create Initial Inventory with 4 Random Card (if player can't have Any Card from Board USe this Cards)
        GiveInitialInventory();

        //send to CardDisplay to Set Display Data For All cards -- whenever we wanna update Display Of cards we need to Invoke This
        onCardDisplay?.Invoke();
    }

    /// <summary>
    /// Before Player Move On Board
    /// </summary>
    #region SET_CARD
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
        for (int i = 0; i < waypoints.cards.Count && i < shuffledCards.Count; i++)
        {
            waypoints.cards[i].Card = shuffledCards[i];
        }
    }

    private void SetNullCards()
    {
        // set null card in main board For Empty card Holder wehn player move
        HashSet<int> uniqueRandomIndices = new HashSet<int>();

        while (uniqueRandomIndices.Count < Null_Card_Amounts)
        {
            int rand = UnityEngine.Random.Range(0, waypoints.cards.Count);
            uniqueRandomIndices.Add(rand);
        }

        Debug.Log("Generated Unique Random Numbers: " + string.Join(", ", uniqueRandomIndices));

        foreach (int index in uniqueRandomIndices)
        {
            if (index >= 0 && index < waypoints.cards.Count && waypoints.cards[index] != null)
            {
                waypoints.cards[index].Card = NullCard;
            }
        }
    }

    #endregion

    #region SET_INITIAL_INVENTORY
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
                uiItemSpawner.SpawnItem(randomCard, false);
                PlayerFightcards.Add(randomCard);
            }
        }
    }

    #endregion


    /// <summary>
    /// Now On Board we Have Dice and Playe Should Click on Dice to Call MOVEPLAYER Function
    /// </summary>
    #region MOVE_PLAYER


    public void MovePlayer(int steps)
    {
        if (remainingMoves <= 0 || isMoving) return;

        remainingMoves--;
        remainingMove_txt.text = remainingMoves.ToString();
        StartCoroutine(MoveOverSteps(steps));
    }


    IEnumerator MoveOverSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % (waypoints.Wayp.Length);

            Vector2 targetPos = (waypoints.Wayp[currentWaypointIndex].GetComponent<RectTransform>().anchoredPosition);

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

        // ********************  MOVE FINISHED   ******************
        if (remainingMoves == 0)
        {
            //MoveFinished.SetActive(true);
            StartCoroutine(Fight());
        }
        // Add an item to inventory AFTER completing all steps
        if (uiItemSpawner != null)
        {
            Card ChosenCard = (waypoints.Wayp[currentWaypointIndex].GetComponentInChildren<CardDisplay>().Card);

            //if card is null don't add to Player Initial Inventory otherwise Add Card To initial Inventory (and Player Fight cards)
            if (ChosenCard.actionType != CardActionType.empty)
            {
                uiItemSpawner.SpawnItem(ChosenCard, false);
                PlayerFightcards.Add(ChosenCard);
                onCardDisplay?.Invoke();
            }
        }

        isMoving = false;
    }

    #endregion


    /// <summary>
    /// After All Move Finished With Dice Go to Fight Section
    /// </summary>

    #region Boss_Fight
    /// Boss Fight Logic ///

    IEnumerator Fight()
    {
        //Disable Board
        MainPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.BossPanel(true);
        yield return new WaitForSeconds(2f);
        UIManager.Instance.BossPanel(false);
        //Enable Boss Fight UI
        BossPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        //Start Boss Fight
        BossFight();
    }

    public void BossFight()
    {
        TurnLoop();
       // StartCoroutine(TurnLoop());

    }
    private void CreateBossInventory()
    {
        if (BossCard == null || uiItemSpawner == null || cardHolder.BossCard.Count < 4)
        {
            Debug.LogWarning("boss Card or UIItemSpawner not assigned properly.");
            return;
        }

        HashSet<int> uniqueIndexes = new HashSet<int>();

        while (uniqueIndexes.Count < 4)
        {
            int randIndex = UnityEngine.Random.Range(0, cardHolder.BossCard.Count);
            uniqueIndexes.Add(randIndex);
        }

        foreach (int index in uniqueIndexes)
        {
            Card randomCard = cardHolder.BossCard[index];
            uiItemSpawner.SpawnItem(randomCard, true);

            //send to CardDisplay
            onCardDisplay?.Invoke();
        }
        BossAttackPhase();
    }
    public void TurnLoop()
    {
        if (gameOver)
            return;

        CheckBattleOutcome();

        // Control From UiItem Spawner After Player Inventory Deleted(Player turn Finished) and now Its Boss Turn
        if (playerTurn)
        {
            CheckBossPoisioned();
            StartCoroutine(pTurn());
        }
        else
        {
            CreateBossInventory();
        }

        playerTurn = !playerTurn;
    }
    //this is for Poisoned Boss
    public void ChangeTurn()
    {
        playerTurn = !playerTurn;
    }

    public void PoisendBoss(int remainCount)
    {
        bossPoisendCount+=remainCount;
    }
    public void CheckBossPoisioned()
    {
        if (bossPoisendCount <= 1)
        {
            HealthBar.instance.BossNormalHealthBar();
            return;
        }
        bossPoisendCount--;
        HealthBar.instance.BossTakeDamage(5f);
    }
    public void ProtectPlayer()
    {
        isPlayerProtected = true;
    }

    IEnumerator pTurn()
    {
        UIManager.Instance.PlyerBossTurn(0);
        yield return new WaitForSeconds(1);
        CreatePlayerAttackInventory();
    }
    public void CreatePlayerAttackInventory()
    {
        if (PlayerFightcards == null || uiItemSpawner == null || PlayerFightcards.Count < 3)
        {
            Debug.LogWarning("Player Fight Card or UIItemSpawner not assigned properly.");
            return;
        }

        HashSet<int> uniqueIndexes = new HashSet<int>();

        while (uniqueIndexes.Count < 3)
        {
            int randIndex = UnityEngine.Random.Range(0, PlayerFightcards.Count);
            uniqueIndexes.Add(randIndex);
        }

        foreach (int index in uniqueIndexes)
        {
            Card randomCard = PlayerFightcards[index];

            uiItemSpawner.SpawnFightCardItem(randomCard);

            //send to CardDisplay
            onCardDisplay?.Invoke();
        }

    }

    public void ReplaceTwoFightCardsButKeep(Card keepCard)
    {

        if (PlayerFightcards == null || uiItemSpawner == null || PlayerFightcards.Count < 3)
            return;

        HashSet<int> uniqueIndexes = new HashSet<int>();

        // انتخاب دو کارت که با keepCard متفاوت باشند
        while (uniqueIndexes.Count < 2)
        {
            int randIndex = UnityEngine.Random.Range(0, PlayerFightcards.Count);
            if (PlayerFightcards[randIndex] != keepCard)
                uniqueIndexes.Add(randIndex);
        }

        foreach (int index in uniqueIndexes)
        {
            Card randomCard = PlayerFightcards[index];
            uiItemSpawner.SpawnFightCardItem(randomCard);
        }

        onCardDisplay?.Invoke();
    }

    public void ReplaceTwoCards()
    {     
            onCardDisplay?.Invoke();
    }


    //Call from UI (Click on card And call Zoom Script)
    public void PlayerAttack(Card mycard)
    {
        if (gameOver)
            return;

        CardEffectManager.Instance.ApplyCardEffect(mycard);
    }

    public void SendEndAction(bool isBoosTurnSkip)
    {
        //Send to UiItemSpawner to Destroy All Spawned card
        //onDestroyPlayedCard?.Invoke(isBoosTurnSkip);
        UiItemSpawner.Instance.DestroyPlayerInventory(isBoosTurnSkip);
    }

    public void BossAttackPhase()
    {
        //go to UiItemSpawner to control Boss Fight Logic
        //onBossAttackTurn?.Invoke();
        UiItemSpawner.Instance.BossAttack();
    }
    public void BossAttackPlayer(int dmg)
    {
        if (isPlayerProtected)
        {
            StartCoroutine(BossAttack(dmg - 5));
            isPlayerProtected= false;   
        }
        else
        {
            StartCoroutine(BossAttack(dmg));
        }
    }
    public IEnumerator BossAttack(int damage)
    {
        HealthBar.instance.PlayerTakeDamage(damage);
        yield return new WaitForSeconds(2f);
        //onDestroyBosscard?.Invoke();
        UiItemSpawner.Instance.DestroyBossInventory();
        ///   TurnLoop();
        ///   
        UIManager.Instance.Player_turn_Over_button_On();

    }


    public void CheckBattleOutcome()
    {
        if (HealthBar.instance.BosscurrentHealth <= 0)
        {
            gameOver = true;
            Debug.Log("Player Wins!");
            UIManager.Instance.Win();
            ScoreManager.Instance.AddScore(50);
            return;
            // Show win screen or handle victory
        }
        else if (HealthBar.instance.PlayercurrentHealth <= 0)
        {
            gameOver = true;
            UIManager.Instance.Gameover();
            Debug.Log("Boss Wins!");
            return;
            // Show lose screen or handle defeat
        }
    }

    #endregion
}
