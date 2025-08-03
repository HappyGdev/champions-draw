using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class CreateTradeUI : MonoBehaviour
{
    [SerializeField] CardDisplay offerCardImage;
    [SerializeField] CardDisplay requestCardImage;
    [SerializeField] TMP_Dropdown offerDropdown;
    [SerializeField] TMP_Dropdown requestDropdown;

    [SerializeField] TradeManager tradeManager;

    [SerializeField] CardHolder cardHolder;

    [SerializeField] PlayerCardsDataManager playerCardsDataManager;

    List<Card> playerCards;       
    List<Card> allCardsInGame;    

    [SerializeField] Button createTradeButton;

    Card currentOfferCard;
    Card currentRequestCard;

    [SerializeField] GameObject notifPanel;

    private void OnEnable()
    {
        Debug.Log("FOUNDTRADES");

        playerCardsDataManager.OnPlayerDataLoaded += Setup;
        playerCardsDataManager.LoadPlayerData();

        tradeManager.OnTradeCreated += EnableSucceedPanel;
    }
    private void OnDisable()
    {
        playerCardsDataManager.OnPlayerDataLoaded -= Setup;

        tradeManager.OnTradeCreated -= EnableSucceedPanel;
    }

    void Setup()
    {
        playerCards = cardHolder.PlayerAvaiableCards;
        allCardsInGame = cardHolder.allCardsInGame;
        // Populate Offer dropdown with player's own cards
        offerDropdown.ClearOptions();


        offerDropdown.AddOptions(playerCards.ConvertAll(card => card.name));
        offerDropdown.onValueChanged.AddListener(OnOfferCardSelected);

        // Populate Request dropdown with all cards in the game
        requestDropdown.ClearOptions();
        requestDropdown.AddOptions(allCardsInGame.ConvertAll(card => card.name));
        requestDropdown.onValueChanged.AddListener(OnRequestCardSelected);


        if (playerCards.Count > 0) OnOfferCardSelected(0);
        if (allCardsInGame.Count > 0) OnRequestCardSelected(0);
    }
   
    void OnOfferCardSelected(int index)
    {
        offerCardImage.Card = playerCards[index];
        offerCardImage.DisplayCard();
        currentOfferCard = playerCards[index];
        createTradeButton.interactable = true;
    }

    void OnRequestCardSelected(int index)
    {
        requestCardImage.Card = allCardsInGame[index];
        requestCardImage.DisplayCard();
        currentRequestCard = allCardsInGame[index];
        createTradeButton.interactable = true;
    }

    public void OnCreateTradeBtnClicked() 
    {
        tradeManager.SaveTradeOffer(currentOfferCard.cardId, currentRequestCard.cardId);
        createTradeButton.interactable = false;

        playerCardsDataManager.LoadPlayerData();

    }
    public void EnableSucceedPanel()
    {
        StopAllCoroutines();
        StartCoroutine(EnableNotif());
    }

  

    IEnumerator EnableNotif()
    {
        notifPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        notifPanel.SetActive(false);
    }
}

