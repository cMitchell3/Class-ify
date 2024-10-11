using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class SignUpInputManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Register")]
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confirmPasswordInputField;
    public TMP_Text warningRegisterText;
    public TMP_Text confirmationText;

    private void Start()
    {
        //Access the Firebase variables from the LoginInputManager
        dependencyStatus = LoginInputManager.dependencyStatus;
        auth = LoginInputManager.auth;
        User = LoginInputManager.User;
    }

    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailInputField.text, passwordInputField.text, usernameInputField.text));
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {

        }
        else if (passwordInputField.text != confirmPasswordInputField.text)
        {

        }
        else
        {
            //Call the Firebase auth sign in function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait unti the task completes
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
                        message = "Weak Password. Password must be at least 6 characters";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use. Please Login Instead";
                        break;
                }
                warningRegisterText.text = message;
                if (warningRegisterText.text.Equals("Email Already In Use. Please Login Instead"))
                {
                    yield return new WaitForSeconds(2);
                    SceneManager.LoadScene("LoginMenuScene");
                }
            }
            else
            {
                //User has been created, get the result
                AuthResult result = RegisterTask.Result;
                User = result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait unti the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set, now return to login screen
                        warningRegisterText.text = "";
                        confirmationText.text = "Account Created Successfully. Please Login Now";
                        yield return new WaitForSeconds(2);
                        SceneManager.LoadScene("LoginMenuScene");
                    }

                }
            }
        }
    }



    //public Button createAccountButton;

    /*
    Errors
    public TMP_Text noMatchMessage;


    void Start()
    {
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInputField.contentType = TMP_InputField.ContentType.Password;
        emailInputField.contentType = TMP_InputField.ContentType.EmailAddress;

        createAccountButton = GameObject.FindGameObjectWithTag("CreateAccountButton").GetComponent<Button>();
        createAccountButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPasswords();
        CheckUsername();
        CheckEmail();
    }

    void CheckPasswords()
    {
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
        {
            if (password != confirmPassword)
            {
                noMatchMessage.text = "Passwords do not match!";
                noMatchMessage.color = Color.red;
                createAccountButton.interactable = false;
            }
            else
            {
                noMatchMessage.text = "Passwords match!";
                noMatchMessage.color = Color.green;
                createAccountButton.interactable = true;
            }
        }
        else
        {
            noMatchMessage.text = "";
            createAccountButton.interactable = false;
        }
    }

    void CheckUsername()
    {
        string username = usernameInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            createAccountButton.interactable = false;
        }
        else
        {
            createAccountButton.interactable = true;
        }
    }

    void CheckEmail()
    {
        string email = emailInputField.text;

        if (string.IsNullOrEmpty(email))
        {
            createAccountButton.interactable = false;
        }
        else
        {
            createAccountButton.interactable = true;
        }
    }
*/
}