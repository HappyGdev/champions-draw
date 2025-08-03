using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvailableTradesUI : MonoBehaviour
{
    [SerializeField] TradeCardElementUI tradeCardELementPrefab;
    [SerializeField] Transform tradeCardsParent;

    [SerializeField] CardDisplay currentOfferCard;
    [SerializeField] CardDisplay currentRequestCard;
    [SerializeField] Button tradeBtn;

    [SerializeField] TradeManager tradeManager;

    [SerializeField] PlayerCardsDataManager cardsDataManager;


    string currentOfferCardId;
    string currentRequestCardId;
    string currentOffererId;
    string currentAcceptorId;
    string currentTradeId;

    [SerializeField] GameObject notifPanel;

    [SerializeField] string notifPanelSucceedText;
    [SerializeField] string notifPanelFailText;

    [SerializeField] TextMeshProUGUI notifPanelText;

    private void OnEnable()
    {
        Debug.Log("FOUNDTRADES");
        tradeManager.OnTradesLoaded += Setup;
        tradeManager.LoadAllOffers();

        tradeManager.OnTradeSucceded += EnableSucceedPanel;
        tradeManager.OnTradeFailed += EnableFailPanel;

       
    }

    
    private void OnDisable()
    {
        tradeManager.OnTradesLoaded -= Setup;

        tradeManager.OnTradeSucceded -= EnableSucceedPanel;
        tradeManager.OnTradeFailed -= EnableFailPanel;
    }


    public void Setup(List<TradeOffer> allTradeOffers, int maxResults)
    {
        Debug.Log("Traded loaded");
        foreach (Transform c in tradeCardsParent)
        {
            Destroy(c.gameObject);
        }

        int numberOfIterations = allTradeOffers.Count <= maxResults ? allTradeOffers.Count : maxResults;

        for (int i = 0; i < numberOfIterations; i++)
        {
            TradeOffer trade = allTradeOffers[i];
            var tradeElement = Instantiate(tradeCardELementPrefab, tradeCardsParent);

            tradeElement.GetComponent<TradeCardElementUI>().Setup(trade.offerCardId, trade.reauestedCardId, trade.userId, cardsDataManager.playerData.userId, trade.tradeId, tradeManager, this);

            tradeElement.SetupOfferCard(trade.offerCardId);

        }

        SetCurrentTrade(allTradeOffers[0].offerCardId, allTradeOffers[0].reauestedCardId, allTradeOffers[0].userId, cardsDataManager.playerData.userId, allTradeOffers[0].tradeId);

    }

    public void SetCurrentTrade(string offerCardId, string requestCardId, string offerId, string acceptorId, string tradeId)
    {
        Debug.Log("Showing cards");

        currentOfferCard.Card = Card.GetFromID(offerCardId);
        currentRequestCard.Card = Card.GetFromID(requestCardId);

        currentOfferCard.DisplayCard();
        currentRequestCard.DisplayCard();

        currentOfferCardId = offerCardId;
        currentRequestCardId = requestCardId;

        currentOffererId = offerId;
        currentAcceptorId = acceptorId;
        currentTradeId = tradeId;

        tradeBtn.interactable = true;

    }

    public void Trade()
    {
        if (currentOfferCardId == null) { return; }
        tradeManager.SwapCards(currentOfferCardId, currentRequestCardId, currentOffererId, currentAcceptorId, currentTradeId);
        tradeBtn.interactable = false;
    }

    public void EnableSucceedPanel()
    {
        StopAllCoroutines();
        StartCoroutine(EnableNotif(notifPanelSucceedText));
    }

    public void EnableFailPanel()
    {
        StopAllCoroutines();
        StartCoroutine(EnableNotif(notifPanelFailText));
    }

    IEnumerator EnableNotif(string notifText)
    {
        notifPanelText.text = notifText;
        notifPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        notifPanel.SetActive(false);
    }
}
