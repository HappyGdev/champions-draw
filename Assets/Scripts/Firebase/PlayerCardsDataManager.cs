using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsDataManager : MonoBehaviour
{
    DatabaseReference dbRef;

    public PlayerCardsData playerData;

    [SerializeField] CardHolder cardHolder;

    private const string PlayerIDKey = "PlayerID";


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


        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase database initialized successfully.");

                LoadPlayerData();
            }
            else
            {
                Debug.LogError("Firebase database initialization failed.");
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadPlayerData();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SavePlayerData();
        }
    }
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
            Debug.Log("PROCESS Fault");
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

        }
        else
        {
            Debug.Log("no data found");
        }
    }
    public void LoadPlayerData()
    {
        StartCoroutine(LoadPlayerDataIE());
    }
}
[System.Serializable]
public class PlayerCardsData
{
    public string userId;
    public List<string> playerCards;
}
