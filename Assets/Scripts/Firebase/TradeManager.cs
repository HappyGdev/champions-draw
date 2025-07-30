using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using System.Collections;
using System.Threading.Tasks;

public class TradeManager : MonoBehaviour
{
    public DatabaseReference dbRef;

    [SerializeField] List<TradeOffer> myTradeOffers = new List<TradeOffer>();

    [SerializeField] List<TradeOffer> allTradeOffers = new List<TradeOffer>();

    [SerializeField] TradeCardElementUI tradeCardELementPrefab;
    [SerializeField] Transform tradeCardsParent;

    [SerializeField] PlayerCardsDataManager cardsDataManager;

    [Header("Test Other Player")]
    [SerializeField] PlayerCardsData otherPlayerData;
    [SerializeField] TradeOffer otherPlayerOffer;

    [SerializeField] List<TradeOffer> otherPlayerOffers = new List<TradeOffer>();

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase database initialized successfully.");

                LoadAllOffers();
            }
            else
            {
                Debug.LogError("Firebase database initialization failed.");
            }
        });


    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveOffer();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadAllOffers();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveOtherPlayerData();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SaveOtherPlayerOffer();
        }
    }

    IEnumerator SwapCardsIE(string offererCardId, string requestedCardId, string offererId, string acceptorId, string tradeId)
    {
        Task<bool> task = ExecuteTrade(offererCardId, requestedCardId, offererId, acceptorId, tradeId);

        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            Debug.Log("ERROR SWAPPING CARDS");
        }
        else
        {
            Debug.Log("SWAPPING CARDS");
        }
    }

    //test should remove
    void SaveOtherPlayerData()
    {
        string json = JsonUtility.ToJson(otherPlayerData);
        dbRef.Child("players").Child(otherPlayerData.userId).SetRawJsonValueAsync(json);
    }

    void SaveOtherPlayerOffer()
    {
        StartCoroutine(SaveOtherPlayerOfferIE());
    }

    IEnumerator SaveOtherPlayerOfferIE()
    {
        yield return LoadOtherPlayerOffersIE();

        // Should change
        TradeOffer tradeOffer = new TradeOffer(otherPlayerData.userId,
            "2ec47c6c-2bbe-4da4-9613-08801beffe17",
            "2c265c10-46ee-4371-ad48-0b208ac2aaed");

        otherPlayerOffers.Add(tradeOffer);

        string json = JsonUtility.ToJson(new TradeListWrapper { trades = otherPlayerOffers });

        // dbRef.Child("allTrades").SetRawJsonValueAsync(json);

        dbRef.Child("allTrades").Child(otherPlayerData.userId).Child(tradeOffer.tradeId).SetRawJsonValueAsync(json);

    }

    void SaveOffer()
    {
        StartCoroutine(SaveOfferIE());
    }

    IEnumerator SaveOfferIE()
    {
        yield return LoadMyOffersIE();

        // Should change
        TradeOffer tradeOffer = new TradeOffer(cardsDataManager.playerData.userId,
            "19dee57e-6070-4174-b8f2-2d71bfbde234",
            "2c265c10-46ee-4371-ad48-0b208ac2aaed");

        myTradeOffers.Add(tradeOffer);

        string json = JsonUtility.ToJson(new TradeListWrapper { trades = myTradeOffers });

        // dbRef.Child("allTrades").SetRawJsonValueAsync(json);

        dbRef.Child("allTrades").Child(cardsDataManager.playerData.userId).Child(tradeOffer.tradeId).SetRawJsonValueAsync(json);

    }

    public void SwapCards(string offerCardId, string requestCardId, string offererId, string acceptorId, string tradeId)
    {
        // should change and replace acceptorId
        StartCoroutine(SwapCardsIE(offerCardId, requestCardId, offererId, otherPlayerData.userId, tradeId));

    }

    [System.Serializable]
    public class TradeListWrapper
    {
        public List<TradeOffer> trades;
    }
    public void LoadAllOffers()
    {
        StartCoroutine(LoadAllOffersIE());
    }

    IEnumerator LoadAllOffersIE()
    {
        var serverData = dbRef.Child("allTrades").GetValueAsync();

        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        if (serverData.IsFaulted)
        {
            Debug.Log("Load all offers PROCESS Fault");
        }

        var resultList = new List<DataSnapshot>();


        allTradeOffers.Clear();

        foreach (var childRoot in serverData.Result.Children)
        {
            foreach (var child in childRoot.Children)
            {
                resultList.Add(child);
                string jsonData = child.GetRawJsonValue();


               // Debug.Log("COUNT " + serverData.Result.Children);

                if (jsonData != null)
                {
                    Debug.Log("server data found");


                    TradeListWrapper wrapper = JsonUtility.FromJson<TradeListWrapper>(jsonData);



                    foreach (var offer in wrapper.trades)
                    {
                        allTradeOffers.Add(offer);
                        Debug.Log("OfferUserName" + offer.userId);
                    }

                    foreach (Transform c in tradeCardsParent)
                    {
                        Destroy(c.gameObject);
                    }

                    foreach (var trade in allTradeOffers)
                    {
                        var tradeElement = Instantiate(tradeCardELementPrefab, tradeCardsParent);

                        tradeElement.GetComponent<TradeCardElementUI>().Setup(trade.offerCardId, trade.reauestedCardId, trade.userId, cardsDataManager.playerData.userId, trade.tradeId, this);
                    }

                    // tradeOffer = JsonUtility.FromJson<TradeOffer>(jsonData);
                }
                else
                {
                    Debug.Log("no data found");
                }
            }
           
        }


        // DataSnapshot snapshot = serverData.Result;
        // string jsonData = snapshot.GetRawJsonValue();

        //  Debug.Log(jsonData);

        //if (jsonData != null)
        //{
        //    Debug.Log("server data found");

        //    allTradeOffers.Clear();

        //    TradeListWrapper wrapper = JsonUtility.FromJson<TradeListWrapper>(jsonData);


        //    Debug.Log("COUNT " + wrapper.trades.Count);

        //    foreach (var offer in wrapper.trades)
        //    {
        //        allTradeOffers.Add(offer);
        //        Debug.Log("OfferUserName" + offer.userId);
        //    }

        //    foreach (Transform child in tradeCardsParent)
        //    {
        //        Destroy(child.gameObject);
        //    }

        //    foreach (var trade in allTradeOffers)
        //    {
        //        var tradeElement = Instantiate(tradeCardELementPrefab, tradeCardsParent);

        //        tradeElement.GetComponent<TradeCardElementUI>().Setup(trade.offerCardId, trade.reauestedCardId, trade.userId, cardsDataManager.playerData.userId, this);
        //    }

        //    // tradeOffer = JsonUtility.FromJson<TradeOffer>(jsonData);
        //}
        //else
        //{
        //    Debug.Log("no data found");
        //}
    }

    IEnumerator LoadMyOffersIE()
    {
        var serverData = dbRef.Child("allTrades").Child(cardsDataManager.playerData.userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        if (serverData.IsFaulted)
        {
            Debug.Log("PROCESS Fault");
        }

        Debug.Log("Loading Player Cards PROCESS COMPLETED");


        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");

            myTradeOffers.Clear();

            TradeListWrapper wrapper = JsonUtility.FromJson<TradeListWrapper>(jsonData);

            foreach (var offer in wrapper.trades)
            {
                myTradeOffers.Add(offer);
                Debug.Log("OfferUserName" + offer.userId);
            }

        }
        else
        {
            Debug.Log("no data found");
        }
    }

    //Test
    IEnumerator LoadOtherPlayerOffersIE()
    {
        var serverData = dbRef.Child("allTrades").Child(otherPlayerData.userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        if (serverData.IsFaulted)
        {
            Debug.Log("Load all offers PROCESS Fault");
        }

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");

            otherPlayerOffers.Clear();

            TradeListWrapper wrapper = JsonUtility.FromJson<TradeListWrapper>(jsonData);

            foreach (var offer in wrapper.trades)
            {
                otherPlayerOffers.Add(offer);
                Debug.Log("OfferUserName" + offer.userId);
            }

        }
        else
        {
            Debug.Log("no data found");
        }
    }


    [System.Serializable]
    public class TradeOffer
    {
        public string tradeId = System.Guid.NewGuid().ToString();
        public string userId;
        public string offerCardId;
        public string reauestedCardId;

        public TradeOffer(string userId, string offerCardId, string reauestedCardId)
        {
            this.userId = userId;
            this.offerCardId = offerCardId;
            this.reauestedCardId = reauestedCardId;
        }
    }



    [ContextMenu("Generate Id")]
    void GenerateId()
    {
        otherPlayerData.userId = System.Guid.NewGuid().ToString();
    }

    public async Task<bool> ExecuteTrade(string offererCardId, string requestedCardId, string offererId, string acceptorId, string tradeId)
    {
        // Get offerer's cards
        var snapshot = await dbRef.Child("players").Child(offererId).GetValueAsync();
        string jsonData = snapshot.GetRawJsonValue();

        PlayerCardsData offererData = JsonUtility.FromJson<PlayerCardsData>(jsonData);


        var acceptorSnapshot = await dbRef.Child("players").Child(acceptorId).GetValueAsync();
        string acceptorJsonData = acceptorSnapshot.GetRawJsonValue();

        PlayerCardsData acceptorData = JsonUtility.FromJson<PlayerCardsData>(acceptorJsonData);



        // Verify the players actually have the cards to trade
        if (!offererData.playerCards.Contains(offererCardId) || !acceptorData.playerCards.Contains(requestedCardId))
        {
            Debug.Log("COULDNT SWAP");
            return false; // Invalid trade
        }



        offererData.playerCards.Remove(offererCardId);
        acceptorData.playerCards.Remove(requestedCardId);

        offererData.playerCards.Add(requestedCardId);
        acceptorData.playerCards.Add(offererCardId);



        // Update Firebase
        string json = JsonUtility.ToJson(offererData);
        var offererUpdateTask = dbRef.Child("players").Child(offererData.userId).SetRawJsonValueAsync(json);

        string json2 = JsonUtility.ToJson(acceptorData);
        var acceptorUpdateTask = dbRef.Child("players").Child(acceptorData.userId).SetRawJsonValueAsync(json2);


        await Task.WhenAll(offererUpdateTask, acceptorUpdateTask);

        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                FirebaseDatabase.DefaultInstance.GetReference("allTrades").Child(offererData.userId).Child(tradeId).RemoveValueAsync();
                Debug.Log("Removed trade successfully.");
            }
            else
            {
                Debug.LogError("Coudnt remove trade");
            }
        });

        cardsDataManager.LoadPlayerData();

        Debug.Log("SWAP COMPLETED");
        return true; // Trade successful
    }
}




