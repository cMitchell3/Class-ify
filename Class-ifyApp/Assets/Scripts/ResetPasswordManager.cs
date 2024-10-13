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

public class ResetPasswordManager : MonoBehaviour
{

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    //Reset Password variables
    [Header("Reset Password")]
    public TMP_InputField forgotPasswordField;
    public TMP_Text warningText;
    public TMP_Text confirmationText;

    // Start is called before the first frame update
    void Start()
    {
        //Access the Firebase variables from the LoginInputManager
        dependencyStatus = LoginInputManager.dependencyStatus;
        auth = LoginInputManager.auth;
        User = LoginInputManager.User;
    }

    public void forgotPasswordButton()
    {
        if (string.IsNullOrEmpty(forgotPasswordField.text))
        {
            warningText.text = "Missing email";
            return;
        }
        forgotPasswordSubmit(forgotPasswordField.text);
    }

    // Method to check if the email exists in Firebase
    void CheckIfEmailExists(string email)
    {
        auth.FetchProvidersForEmailAsync(email).ContinueWithOnMainThread((task) =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                // Log the error for debugging
                Debug.LogError("Error fetching providers for email: " + task.Exception.ToString());
                warningText.text = "Error occurred while checking email. Please try again.";
                confirmationText.text = "";
                return;
            }

            var providers = new List<string>(task.Result);

            // Check if the email has associated sign-in methods (exists in Firebase)
            if (providers.Count > 0)
            {
                // Email exists, proceed with sending the reset password email
                forgotPasswordSubmit(email);
            }
            else
            {
                // No providers, meaning the email does not exist
                warningText.text = "The email does not exist. Please try again.";
                confirmationText.text = "";
            }
        });
    }

    // Method to send a password reset email
    void forgotPasswordSubmit(string forgotPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgotPasswordEmail).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync was cancelled or failed.");
                warningText.text = "The email does not exist. Please try again.";
                confirmationText.text = "";
                return;
            }

            // Successfully sent the password reset email
            warningText.text = "";
            confirmationText.text = "Password reset email sent successfully.";
        });
    }
}
