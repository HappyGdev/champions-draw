using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCardsDataManager : MonoBehaviour
{
    DatabaseReference dbRef;

    public PlayerCardsData playerData;

    [SerializeField] CardHolder cardHolder;

    private const string PlayerIDKey = "PlayerID";

    public Action OnPlayerDataLoaded;
    public Action<DatabaseReference> OnDBRefLoaded;


    void Start()
    {
        if (!PlayerPrefs.HasKey(PlayerIDKey))
        {
            string uniqueID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerIDKey, uniqueID);
            PlayerPrefs.Save();
            playerData.userId = uniqueID; 
        }

        playerData.userId = PlayerPrefs.GetString(PlayerIDKey);

        Debug.Log("Player ID: " + playerData.userId);

        LoadFirebaseRef();

        //FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task => {
        //    if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
        //    {
        //        Debug.Log("Signed in anonymously! UID: " + task.Result.User.UserId);

        //    }

        //    else
        //        Debug.LogError("Anonymous sign-in failed: " + task.Exception);
        //});


    }

    void LoadFirebaseRef()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase database initialized successfully.");

                OnDBRefLoaded?.Invoke(dbRef);

                LoadPlayerData();
            }
            else
            {
                Debug.LogError("Firebase database initialization failed.");
            }
        });
    }
   
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        LoadPlayerData();
    //    }
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        SavePlayerData();
    //    }
    //}
    void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData);
        dbRef.Child("players").Child(playerData.userId).SetRawJsonValueAsync(json);
    }
    IEnumerator LoadPlayerDataIE()
    {
        var serverData = dbRef.Child("players").Child(playerData.userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        if (serverData.IsFaulted)
        {
            Debug.Log("PROCESS Fault Loading player data");
        }

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");

            playerData.playerCards.Clear();

            var data = JsonUtility.FromJson<PlayerCardsData>(jsonData);

            foreach (var card in data.playerCards)
            {
                playerData.playerCards.Add(card);
            }

            cardHolder.UpdatePlayerAvailableCards(playerData.playerCards);

            OnPlayerDataLoaded?.Invoke();
        }
        else
        {
            AddStarterPackCards();
            cardHolder.UpdatePlayerAvailableCards(playerData.playerCards);
            SavePlayerData();
            Debug.Log("no data found, saving current one");
        }
    }
    public void LoadPlayerData()
    {
        StartCoroutine(LoadPlayerDataIE());
    }

    [ContextMenu("AddStarterPackCards")] 
    public void AddStarterPackCards()
    {
        if(playerData.playerCards.Count > 0) { return; }

        foreach(var card in cardHolder.StarterCards)
        {
            playerData.playerCards.Add(card.cardId);
        }
    }


}
[System.Serializable]
public class PlayerCardsData
{
    public string userId;
    public List<string> playerCards;
}
