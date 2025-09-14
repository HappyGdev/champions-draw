using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    public static Action<int> onBuycomplete;
    #region Firebase Variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    #endregion

    #region Login/Register UI
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public TextMeshProUGUI InfoText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    [Space]
    public TMP_InputField userProfileRegisterNumber;

    [Header("UI Panels")]
    public GameObject dataPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject mainSignPanel;
    public TextMeshProUGUI[] inGameName;
    public Button profile_Button;
    #endregion

    #region User Data UI
    [Header("User Data")]
    public TMP_InputField usernameField;
    //public TMP_InputField scoreField;
    //public TMP_InputField killsField;
    //public TMP_InputField deathsField;
    //public TMP_InputField userProfileNumber_txt;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    public int coin;
    public TMP_Text coinText;


    #endregion

    [Header("Auto-Login")]
    public TMP_Text autoLoginText; // Login UI and Text
    private bool isCheckingAutoLogin = false;

    #region Shop
    [Header("Shop")]
    public ShopManager shopManager;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Initialize Firebase when the game starts
        StartCoroutine(CheckAndInitializeFirebase());
    }

    private IEnumerator Start()
    {
        PlayerPrefs.SetInt("Badge1", 0);
        PlayerPrefs.SetInt("Badge2", 0);
        profile_Button.interactable = false;


        while (auth == null)
            yield return null;

        // شروع auto-login
        isCheckingAutoLogin = true;
        if (autoLoginText != null)
            autoLoginText.text = "Checking login...";

        // Guest login
        if (PlayerPrefs.HasKey("UserProfileNumnber") && PlayerPrefs.GetInt("UserProfileNumnber") == 0)
        {
            AutoGuestLogin();
            isCheckingAutoLogin = false;
            if (autoLoginText != null) autoLoginText.text = "";
            yield break;
        }

        // Auto login با استفاده از اطلاعات ذخیره شده
        if (PlayerPrefs.HasKey("SavedEmail") && PlayerPrefs.HasKey("SavedPassword"))
        {
            string savedEmail = PlayerPrefs.GetString("SavedEmail");
            string savedPassword = PlayerPrefs.GetString("SavedPassword");

            yield return StartCoroutine(Login(savedEmail, savedPassword));
        }
        // اگر کاربر فعلی Firebase موجود است
        else if (auth.CurrentUser != null)
        {
            User = auth.CurrentUser;
            yield return StartCoroutine(LoadUserData());
            SetUIAfterLogin(User.DisplayName);
        }
        else
        {
            loginPanel.SetActive(true);
        }

        isCheckingAutoLogin = false;
        if (autoLoginText != null) autoLoginText.text = "";
    }
    //public int GetCoin()
    //{
    //    return coin;
    //}
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && User != null)
    //    {
    //        coin += 10;
    //        Debug.Log("Coin increased to: " + coin);
    //        coinText.text = "Coin: " + coin;
    //        StartCoroutine(UpdateCoin(coin));
    //    }
    //}
    public void SetCoin(int absoluteAmount)
    {
        StartCoroutine(SetCoinCoroutine(absoluteAmount));
    }

    private IEnumerator SetCoinCoroutine(int newAmount)
    {
        var setCoinTask = DBreference.Child("users").Child(User.UserId).Child("coin").SetValueAsync(newAmount);
        yield return new WaitUntil(() => setCoinTask.IsCompleted);

        if (setCoinTask.Exception != null)
        {
            Debug.LogWarning("Failed to set coin: " + setCoinTask.Exception);
            InfoText.text = setCoinTask.Exception.Message;
            yield return new WaitForSeconds(2f);
            InfoText.text = " ";
        }
        else
        {
            coin = newAmount;
            coinText.text = "Coin: " + coin;
            PlayerPrefs.SetInt("Coin", coin);
            onBuycomplete?.Invoke(coin);
            Debug.Log($"[SetCoin] Coin set to {coin}");
        }
    }
    public void AddCoins(int amountToAdd)
    {
        StartCoroutine(AddCoinsCoroutine(amountToAdd));
    }

    private IEnumerator AddCoinsCoroutine(int amount)
    {
        var getCoinTask = DBreference.Child("users").Child(User.UserId).Child("coin").GetValueAsync();
        yield return new WaitUntil(() => getCoinTask.IsCompleted);

        if (getCoinTask.Exception != null)
        {
            Debug.LogWarning("Failed to retrieve current coin: " + getCoinTask.Exception);
            yield break;
        }

        int currentCoin = 0;
        if (getCoinTask.Result.Exists && getCoinTask.Result.Value != null)
            currentCoin = Convert.ToInt32(getCoinTask.Result.Value);

        int newCoin = currentCoin + amount;

        var setCoinTask = DBreference.Child("users").Child(User.UserId).Child("coin").SetValueAsync(newCoin);
        yield return new WaitUntil(() => setCoinTask.IsCompleted);

        if (setCoinTask.Exception != null)
        {
            Debug.LogWarning("Failed to add coin: " + setCoinTask.Exception);
        }
        else
        {
            coin = newCoin;
            coinText.text = "Coin: " + coin;
            PlayerPrefs.SetInt("Coin", coin);
            onBuycomplete?.Invoke(coin);
            Debug.Log($"[AddCoins] Coin updated: +{amount}, total: {coin}");
        }
    }
    //public void SetAndSaveCoin(int amount)
    //{
    //    Debug.Log("Coin Updated" + amount);
    //    StartCoroutine(UpdateCoin(amount));
    //}
    //private IEnumerator UpdateCoin(int valueToAdd)
    //{
    //    // Step 1: دریافت مقدار فعلی coin از Firebase
    //    var getCoinTask = DBreference.Child("users").Child(User.UserId).Child("coin").GetValueAsync();
    //    yield return new WaitUntil(() => getCoinTask.IsCompleted);

    //    if (getCoinTask.Exception != null)
    //    {
    //        Debug.LogWarning("Failed to retrieve current coin: " + getCoinTask.Exception);
    //        InfoText.text = getCoinTask.Exception.Message;
    //        yield return new WaitForSeconds(2f);
    //        InfoText.text = " ";
    //        yield break;
    //    }

    //    int currentCoin = 0;
    //    if (getCoinTask.Result.Exists && getCoinTask.Result.Value != null)
    //    {
    //        currentCoin = Convert.ToInt32(getCoinTask.Result.Value);
    //    }

    //    // محاسبه مقدار جدید coin
    //    int newCoin = currentCoin + valueToAdd;

    //    //ذخیره مقدار جدید در Firebase
    //    var setCoinTask = DBreference.Child("users").Child(User.UserId).Child("coin").SetValueAsync(newCoin);
    //    yield return new WaitUntil(() => setCoinTask.IsCompleted);

    //    if (setCoinTask.Exception != null)
    //    {
    //        Debug.LogWarning("Failed to update coin: " + setCoinTask.Exception);
    //        InfoText.text = setCoinTask.Exception.Message;
    //        yield return new WaitForSeconds(2f);
    //        InfoText.text = " ";
    //    }
    //    else
    //    {
    //        // Step 4: آپدیت مقدار داخلی (local) و UI
    //        coin = newCoin;
    //        coinText.text = "Coin: " + coin;
    //        PlayerPrefs.SetInt("Coin", coin); // optional: save locally
    //        onBuycomplete?.Invoke(coin);
    //        Debug.Log($"Coin updated and saved: {valueToAdd}, total: {coin}");
    //    }
    //}

    private IEnumerator CheckAndInitializeFirebase()
    {
        // Check Firebase dependencies
        var check = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => check.IsCompleted);

        dependencyStatus = check.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            // Initialize Auth and Realtime Database
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase initialized successfully!");
        }
        else
        {
            Debug.LogError("Could not resolve Firebase dependencies: " + dependencyStatus);
        }
    }

    #region Login/Register/SignOut

    // Called by Login Button
    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        // Firebase login request
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            HandleLoginError(loginTask.Exception);
        }
        else
        {
            // Login successful
            User = loginTask.Result.User;

            // Save credentials for auto-login
            PlayerPrefs.SetString("SavedEmail", _email);
            PlayerPrefs.SetString("SavedPassword", _password);
            PlayerPrefs.Save();
            StartCoroutine(LoadUserData());

            yield return StartCoroutine(LoadUserData()); // ✅ صبر کن تا لود کامل بشه

            yield return StartCoroutine(UpdateEmailDatabase(_email));

            SetUIAfterLogin(User.DisplayName);
        }
    }

    private void HandleLoginError(System.Exception exception)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        string message = "Login Failed!";

        // Handle common Firebase Auth errors
        switch (errorCode)
        {
            case AuthError.MissingEmail: message = "Missing Email"; break;
            case AuthError.MissingPassword: message = "Missing Password"; break;
            case AuthError.WrongPassword: message = "Wrong Password"; break;
            case AuthError.InvalidEmail: message = "Invalid Email"; break;
            case AuthError.UserNotFound: message = "Account does not exist"; break;
        }
        warningLoginText.text = message;
    }

    // Called by Register Button
    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        // Validate inputs
        if (string.IsNullOrEmpty(_username))
        {
            warningRegisterText.text = "Missing Username";
            yield break;
        }
        if (_password != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
            yield break;
        }

        // Firebase create user
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            HandleRegisterError(registerTask.Exception);
        }
        else
        {
            User = registerTask.Result.User;

            // ✅ Update display name in Firebase Auth
            var profileTask = User.UpdateUserProfileAsync(new UserProfile { DisplayName = _username });
            yield return new WaitUntil(() => profileTask.IsCompleted);

            // ✅ Save username to database
            yield return StartCoroutine(UpdateUsernameDatabase(_username));

            // ✅ Save credentials locally
            PlayerPrefs.SetString("SavedEmail", _email);
            PlayerPrefs.SetString("SavedPassword", _password);
            PlayerPrefs.Save();
            yield return StartCoroutine(UpdateEmailDatabase(_email));
            // ✅ Reset shop, then continue
            shopManager.ResetShop();
            SetUIAfterLogin(_username);
        }
    }
    private void HandleRegisterError(System.Exception exception)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        string message = "Register Failed!";

        switch (errorCode)
        {
            case AuthError.MissingEmail: message = "Missing Email"; break;
            case AuthError.MissingPassword: message = "Missing Password"; break;
            case AuthError.WeakPassword: message = "Weak Password"; break;
            case AuthError.EmailAlreadyInUse: message = "Email Already In Use"; break;
        }
        warningRegisterText.text = message;
    }

    // Called by Sign Out Button
    public void SignOutButton()
    {
        if (auth.CurrentUser != null) auth.SignOut();
        if (PlayerPrefs.HasKey("UserProfileNumnber")) PlayerPrefs.DeleteKey("UserProfileNumnber");

        ClearUIAfterLogout();
    }

    private void SetUIAfterLogin(string username)
    {
        foreach (var user in inGameName) user.text = username;
        profile_Button.interactable = true;
        mainSignPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }

    private void ClearUIAfterLogout()
    {
        foreach (var user in inGameName) user.text = "Not Logged In";
        profile_Button.interactable = false;
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        mainSignPanel.SetActive(true);
        ClearLoginFeilds();
        ClearRegisterFeilds();
    }

    #endregion

    #region Guest Login
    public void GuestLogin()
    {
        PlayerPrefs.SetInt("Badge1", 0);
        PlayerPrefs.SetInt("Badge2", 0);
        // Save guest profile number = 0
        PlayerPrefs.SetInt("UserProfileNumnber", 0);
        PlayerPrefs.Save();
        AutoGuestLogin();
    }

    private void AutoGuestLogin()
    {
        foreach (var user in inGameName) user.text = "GUEST";
        profile_Button.interactable = false;
        mainSignPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }
    #endregion

    #region UserData

    public void SaveDataButton()
    {
        var username = PlayerPrefs.GetString("SavedEmail");
        Debug.Log("user name is " +  username); 
        StartCoroutine(UpdateUsernameAuth(username));
        StartCoroutine(UpdateUsernameDatabase(username));
        var myscore = ScoreManager.Instance.GetScore();
        StartCoroutine(UpdateScore(myscore)); 
        //StartCoroutine(UpdateXp(int.Parse(xpField.text)));
        //StartCoroutine(UpdateKills(int.Parse(killsField.text)));
        //StartCoroutine(UpdateDeaths(int.Parse(deathsField.text)));
    }

    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        Task ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateScore(int _score)
    {
        //Set the currently logged in user xp
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("score").SetValueAsync(_score);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
        }
    }

    private IEnumerator UpdateKills(int _kills)
    {
        //Set the currently logged in user kills
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("kills").SetValueAsync(_kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }
    public void SaveUserProfileNumber(int number)
    {
        StartCoroutine(UpdateUserProfileNumber(number));
    }

    private IEnumerator UpdateUserProfileNumber(int number)
    {
        var task = DBreference.Child("users").Child(User.UserId).Child("userProfileNumber").SetValueAsync(number);
        yield return new WaitUntil(() => task.IsCompleted);
        //PlayerPrefs.SetInt("UserProfileNumnber", number);
        shopManager.UpdateUserPhoto(number);
        //PlayerPrefs.Save();
        if (task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {task.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }
    private IEnumerator UpdateDeaths(int _deaths)
    {
        //Set the currently logged in user deaths
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("deaths").SetValueAsync(_deaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    private IEnumerator LoadUserData()
    {
        Debug.Log("Loading User Data");

        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            //scoreField.text = "0";
            //killsField.text = "0";
            //deathsField.text = "0";
            Debug.Log("In Value Null");
            PlayerPrefs.SetInt("Coin", 0);
        }
        else
        {
            Debug.Log("In Value Not Null");

            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Child("coin").Value != null)
            {
                coin = Convert.ToInt32(snapshot.Child("coin").Value);
                coinText.text = coin.ToString();
                Debug.Log("Coin Loaded: " + coin);
                PlayerPrefs.SetInt("Coin",coin);    
            }
            else
            {
                coin = 0;
                coinText.text = coin.ToString();
                Debug.Log("Coin not found, defaulted to 0.");
                PlayerPrefs.SetInt("Coin", 0);
            }
            if (snapshot.Child("unlockedItems").Value != null)
            {
                string jsonArray = snapshot.Child("unlockedItems").GetRawJsonValue();
                int[] unlocked = JsonHelper.FromJson<int>(jsonArray);
                shopManager.LoadUnlockedItems(unlocked);
               
            }
            // ✅ Read userProfileNumber
            if (snapshot.Child("userProfileNumber").Value != null)
            {
                int profileNumber = Convert.ToInt32(snapshot.Child("userProfileNumber").Value);
                Debug.Log("User Profile Number: " + profileNumber);
                shopManager.UpdateUserPhoto(profileNumber);
                // Use profileNumber as needed here...
            }
            else
            {
                shopManager.UpdateUserPhoto(0);
            }
            //scoreField.text = snapshot.Child("score").Value.ToString();
            //killsField.text = snapshot.Child("kills").Value.ToString();
            //deathsField.text = snapshot.Child("deaths").Value.ToString();
            LoadBadges(snapshot);

            coinText.text = "Coin: " + coin; // ✅ مقدار UI را به‌روزرسانی کن

        }


    }

    private IEnumerator LoadScoreboardData()
    {
        // Get all users data ordered by score
        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("score").GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load leaderboard: {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            // Clear existing elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            int rank = 1;

            // Iterate through users in descending score order
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = "Unknown";
                int score = 0;
                int profileNumber = 0;

                // ✅ Use username if available
                if (childSnapshot.HasChild("username") && childSnapshot.Child("username").Value != null)
                {
                    username = childSnapshot.Child("username").Value.ToString();
                }
                else if (childSnapshot.HasChild("email") && childSnapshot.Child("email").Value != null)
                {
                    // ✅ Fallback to email (before '@')
                    string email = childSnapshot.Child("email").Value.ToString();
                    username = email.Contains("@") ? email.Split('@')[0] : email;
                }

                if (childSnapshot.HasChild("score") && childSnapshot.Child("score").Value != null)
                {
                    int.TryParse(childSnapshot.Child("score").Value.ToString(), out score);
                }

                if (childSnapshot.HasChild("userProfileNumber") && childSnapshot.Child("userProfileNumber").Value != null)
                {
                    int.TryParse(childSnapshot.Child("userProfileNumber").Value.ToString(), out profileNumber);
                }

                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, score, profileNumber, rank);
                rank++;
            }

            // Show scoreboard UI panel
            dataPanel.SetActive(true);
        }
    }
    private IEnumerator UpdateEmailDatabase(string email)
    {
        var task = DBreference.Child("users").Child(User.UserId).Child("email").SetValueAsync(email);
        yield return new WaitUntil(() => task.IsCompleted);
    }
    #endregion

    #region Shop
    public void SaveUnlockedItems(ShopItem[] items)
    {
        int[] unlocked = items.Select(i => i.isUnlocked ? 1 : 0).ToArray();
        StartCoroutine(UpdateUnlockedItems(unlocked));
    }

    private IEnumerator UpdateUnlockedItems(int[] unlockedArray)
    {
        string jsonArray = JsonHelper.ToJson(unlockedArray, true);
        var task = DBreference.Child("users").Child(User.UserId).Child("unlockedItems").SetRawJsonValueAsync(jsonArray);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array, bool prettyPrint = false)
        {
            Wrapper<T> wrapper = new Wrapper<T> { Items = array };
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
    #endregion

    #region Utility
    public void ClearLoginFeilds() { emailLoginField.text = ""; passwordLoginField.text = ""; }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }
    #endregion

    #region Badge

    public void SaveBadge(int badgeNumber)
    {
        StartCoroutine(UpdateBadge(badgeNumber));
    }

    private IEnumerator UpdateBadge(int badgeNumber)
    {
        Task task = DBreference.Child("users").Child(User.UserId).Child("badges").Child("badge" + badgeNumber).SetValueAsync(1);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning("Failed to save badge: " + task.Exception);
        }
        else
        {
            Debug.Log($"Badge {badgeNumber} saved successfully.");
        }
    }
    private void LoadBadges(DataSnapshot snapshot)
    {
        Debug.Log("Loading badges");
        DataSnapshot badgesSnapshot = snapshot.Child("badges");    
        if (badgesSnapshot.Exists)
        {
            int badge1 = 0;
            int badge2 = 0;

            if (badgesSnapshot.HasChild("badge1") && badgesSnapshot.Child("badge1").Value != null)
            {
                badge1 = Convert.ToInt32(badgesSnapshot.Child("badge1").Value);
                shopManager.LoadBadges(badge1, 0);
            }
            else
            {
                shopManager.LoadBadges(0, 0);
            }

            if (badgesSnapshot.HasChild("badge2") && badgesSnapshot.Child("badge2").Value != null)
            {
                badge2 = Convert.ToInt32(badgesSnapshot.Child("badge2").Value);
                shopManager.LoadBadges(badge2, 1);
            }
            else
            {
                shopManager.LoadBadges(0, 1);
            }

            Debug.Log("Badge 1: " + badge1);
            Debug.Log("Badge 2: " + badge2);

            // TODO UI
        }
        else
        {
            Debug.Log("badges dont exists");
            shopManager.LoadBadges(0, 0);
            shopManager.LoadBadges(0, 0);

        }

    }


    #endregion
}
