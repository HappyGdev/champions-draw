using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    [Space]
    public TMP_InputField userProfileRegisterNumber;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField xpField;
    public TMP_InputField killsField;
    public TMP_InputField deathsField;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    //public int userProfileNumber;
    public TMP_InputField userProfileNumber_txt;


    [Header("UI")]
    public GameObject dataPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public TextMeshProUGUI[] inGameName;
    public GameObject mainSignPanel;
    public Button profile_Button;


    private bool firebaseReady = false;

    private void Awake()
    {
        StartCoroutine(CheckAndInitializeFirebase());
    }
    private void Start()
    {
        profile_Button.interactable = false;
    }
    private void Update()
    {
        // وقتی E روی کیبورد زده شد
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Loading user data from Firebase...");
            StartCoroutine(LoadUserData());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Loading UserProfileNumber...");
            StartCoroutine(LoadUserProfileNumber());
        }
    }

    private IEnumerator CheckAndInitializeFirebase()
    {
        var check = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => check.IsCompleted);

        dependencyStatus = check.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            // Set up Firebase Auth
            auth = FirebaseAuth.DefaultInstance;

            // Set up Firebase Realtime Database reference
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;

            Debug.Log("Firebase initialized successfully!");
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
    }

    #region User Profile

    // this is my manuell Function to Save USer Profile number
    public void SaveUserProfileNumber(int number)
    {
        StartCoroutine(UpdateUserProfileNumber(number));
    }

    private IEnumerator UpdateUserProfileNumber(int number)
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("userProfileNumber").SetValueAsync(number);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to update UserProfileNumber: {DBTask.Exception}");
        }
    }

    // --- لود کردن UserProfileNumber ---   this is my Manuell Load Profile Data
    private IEnumerator LoadUserProfileNumber()
    {
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).Child("userProfileNumber").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load UserProfileNumber: {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("No UserProfileNumber set yet. Default = 0");
        }
        else
        {
            int number = int.Parse(DBTask.Result.Value.ToString());
            Debug.Log("Loaded UserProfileNumber: " + number);

            // نمایش در UI (فرض کنیم xpField رو تستی استفاده کنیم یا یه TMP_Text جدید تعریف کنی)
            if (userProfileNumber_txt != null)
            {
                userProfileNumber_txt.text = number.ToString();
            }
        }
    }

    #endregion

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();


        //UIManager.instance.LoginScreen();
        loginPanel.SetActive(true);


        ClearRegisterFeilds();
        ClearLoginFeilds();
    }
    //Function for the save button
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        StartCoroutine(UpdateXp(int.Parse(xpField.text)));
        StartCoroutine(UpdateKills(int.Parse(killsField.text)));
        StartCoroutine(UpdateDeaths(int.Parse(deathsField.text)));
        StartCoroutine(UpdateUserProfileNumber(int.Parse(userProfileNumber_txt.text)));  // New User Profile Data

    }
    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            usernameField.text = User.DisplayName;

            foreach (var user in inGameName)
            {
                user.text = User.DisplayName;
            }
            //UIManager.instance.UserDataScreen(); // Change to user data UI
            //loginPanel.SetActive(false);
            profile_Button.interactable = true;
            mainSignPanel.SetActive(false);

           // if we manuelly wanna set Data we can use this field
           // dataPanel.SetActive(true);

            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Register is Fuinished Completely
                        //Username is now set
                        //Now return to login screen


                        //UIManager.instance.LoginScreen();
                        // loginPanel.SetActive(true);
                        foreach (var user in inGameName)
                        {
                            user.text = User.DisplayName;
                        }

                        profile_Button.interactable = true;

                        //Here we Set Panel 
                        //loginPanel.SetActive(true);
                        //registerPanel.SetActive(false);

                        mainSignPanel.SetActive(false);

                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
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

    private IEnumerator UpdateXp(int _xp)
    {
        //Set the currently logged in user xp
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("xp").SetValueAsync(_xp);

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
        if (DBreference == null)
        {
            Debug.LogError("DBreference is NULL. Did you initialize Firebase?");
            yield break;
        }

        if (User == null)
        {
            Debug.LogError("User is NULL. Are you logged in?");
            yield break;
        }

        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to load user data: {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("No data exists yet. Setting defaults.");

            if (xpField != null) xpField.text = "0";
            if (killsField != null) killsField.text = "0";
            if (deathsField != null) deathsField.text = "0";
            if (userProfileNumber_txt != null) userProfileNumber_txt.text = "0";   // 🔥
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            if (xpField != null) xpField.text = snapshot.Child("xp").Value?.ToString() ?? "0";
            if (killsField != null) killsField.text = snapshot.Child("kills").Value?.ToString() ?? "0";
            if (deathsField != null) deathsField.text = snapshot.Child("deaths").Value?.ToString() ?? "0";
            if (userProfileNumber_txt != null) userProfileNumber_txt.text = snapshot.Child("userProfileNumber").Value?.ToString() ?? "0";  // 🔥
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
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                //scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, kills, deaths, xp);
            }

            //Go to scoareboard screen
            //UIManager.instance.ScoreboardScreen();
        }
    }

    #region Guest Login

    public void GuestLogin()
    {

        Debug.Log("Button Clicked");
        profile_Button.interactable = false;
        foreach (var user in inGameName)
        {
            user.text = "GUEST";
        }
        mainSignPanel.SetActive(false);
    }


    #endregion
}
