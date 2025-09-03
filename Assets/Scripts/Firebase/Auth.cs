using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

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
        profile_Button.interactable = false;

        // صبر کن تا Firebase Auth آماده بشه
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

        // پایان بررسی
        isCheckingAutoLogin = false;
        if (autoLoginText != null) autoLoginText.text = "";
    }


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

            // Update display name
            var profileTask = User.UpdateUserProfileAsync(new UserProfile { DisplayName = _username });
            yield return new WaitUntil(() => profileTask.IsCompleted);

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
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //scoreField.text = snapshot.Child("score").Value.ToString();
            //killsField.text = snapshot.Child("kills").Value.ToString();
            //deathsField.text = snapshot.Child("deaths").Value.ToString();
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                //int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                //int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                int score = int.Parse(childSnapshot.Child("score").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, score);
            }

            //Go to scoareboard screen
            //UIManager.instance.ScoreboardScreen();
        }
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
}
