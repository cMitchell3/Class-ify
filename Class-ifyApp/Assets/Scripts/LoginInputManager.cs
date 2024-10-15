using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.AI;
using UnityEngine.SceneManagement;




public class LoginInputManager : MonoBehaviour
{
    // Singleton instance
    //public static LoginInputManager Instance;


    //Firebase variables
    [Header("Firebase")]
    public static DependencyStatus dependencyStatus;
    public static FirebaseAuth auth;
    public static FirebaseUser User;


    [Header("Login")]
    // Input Fields
    public TMP_InputField emailUsernameInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text warningLoginText;


    private void Awake()
    {
        passwordInputField.contentType = TMP_InputField.ContentType.Password;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are available initialize Firebase
                Debug.Log("Firebase is ready");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;


    }


    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailUsernameInputField.text, passwordInputField.text));
    }


    public void RegisterButton()
    {
        SceneManager.LoadScene("SignUpMenuScene");
    }

    public void ForgotPasswordButton()
    {
        SceneManager.LoadScene("ResetPasswordScene");
    }


    private IEnumerator Login(string _email, string _password)
    {
        //Call Firebase auth sign in function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);


        if (LoginTask.Exception != null)
        {
            //If there are errors, handle them
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
            //User is now logged in, now get result
            AuthResult result = LoginTask.Result;
            User = result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            // Change scene upon successful login
            SceneManager.LoadScene("MainMenuScene");


        }
    }
}
