using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeCardElementUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI offerCardIdText;
    [SerializeField] TextMeshProUGUI requestCardIdText;
    [SerializeField] Button tradeButton;

    string offererId;
    string acceptorId;
    string tradeId;

    TradeManager tradeManager;

    public void Setup(string offerCardId, string requestCardId,string offererID, string acceptorID, string tradeID, TradeManager tradeManager)
    {
        offerCardIdText.text = Card.GetFromID(offerCardId).name;
        requestCardIdText.text = Card.GetFromID(requestCardId).name;
        offererId = offererID;
        acceptorId = acceptorID;
        tradeId = tradeID;

        this.tradeManager = tradeManager;

        tradeManager.dbRef.Child("allTrades").Child(offererId).Child(tradeId).ChildRemoved += OnTradeRemoved;

        tradeButton.onClick.AddListener(() =>
        { tradeManager.SwapCards(offerCardId, requestCardId, offererId, acceptorId, tradeId); });
    }

    private void OnTradeRemoved(object sender, ChildChangedEventArgs e)
    {
        try
        {
            Destroy(gameObject);
        }
        catch (System.Exception)
        {
        }
    }
    private void OnDestroy()
    {
        try
        {
            tradeManager.dbRef.Child("allTrades").Child(tradeId).ChildRemoved -= OnTradeRemoved;
        }
        catch (System.Exception)
        {
        }
      
    }
}
