using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{
    private FirebaseAuth auth;

    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SignUp(emailInputField.text, passwordInputField.text);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Login(emailInputField.text, passwordInputField.text);
        }
    }

    public void SignUp(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("User created successfully!");
            }
            else
            {
                Debug.LogError("Error creating user: " + task.Exception);
            }
        });
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("User logged in successfully!");
            }
            else
            {
                Debug.LogError("Error logging in: " + task.Exception);
            }
        });
    }
}