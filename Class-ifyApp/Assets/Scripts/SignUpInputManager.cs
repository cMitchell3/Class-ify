using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SignUpInputManager : MonoBehaviour
{
    // Inputs
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confirmPasswordInputField;
    public Button createAccountButton;

    // Errors
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
}
