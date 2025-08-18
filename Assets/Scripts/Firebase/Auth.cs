using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;

public class Auth : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

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

    private bool firebaseReady = false;

    // Initialize Firebase in Start instead of Awake
    private async void Start()
    {
        var checkDependencies = await FirebaseApp.CheckAndFixDependenciesAsync();
        dependencyStatus = checkDependencies;

        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase dependencies resolved.");
            InitializeFirebase();
            firebaseReady = true;
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    public async void LoginButton()
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        await Login(emailLoginField.text, passwordLoginField.text);
    }

    public async void RegisterButton()
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        await Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text);
    }

    private async Task Login(string _email, string _password)
    {
        try
        {
            var loginTask = await auth.SignInWithEmailAndPasswordAsync(_email, _password);
            User = loginTask.User;

            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
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
            Debug.LogWarning($"Login Failed: {message} ({ex.Message})");
        }
    }

    private async Task Register(string _email, string _password, string _username)
    {
        if (string.IsNullOrEmpty(_username))
        {
            warningRegisterText.text = "Missing Username";
            return;
        }

        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
            return;
        }

        try
        {
            var registerTask = await auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            User = registerTask.User;

            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _username };
                await User.UpdateUserProfileAsync(profile);

                warningRegisterText.text = "";
                Debug.Log("User registered and username set successfully.");
            }
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
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
            Debug.LogWarning($"Register Failed: {message} ({ex.Message})");
        }
    }
}
