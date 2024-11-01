using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class UpdateUsernameLogic : MonoBehaviour
{

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    public TMP_InputField usernameInputField;
    public TMP_Text usernameConfirmationText;

    void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;

        if (db == null)
        {
            Debug.LogError("Error: Failed to connect to Firestore.");
        }
        else
        {
            Debug.Log("Connected to Firestore");
        }

        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        if (auth == null)
        {
            Debug.LogError("Error: Failed to connected to Firebase Auth");
        }

        if (user == null)
        {
            Debug.LogError("Error: User not logged in.");
        }
    }

    public void UpdateUsername()
    {
        //Store the username in the database
        DocumentReference docRef = db.Collection("user").Document(user.Email);
        Dictionary<string, object> userUsername = new Dictionary<string, object>
                    {
                        { "username", usernameInputField.text },
                    };
        docRef.SetAsync(userUsername).ContinueWithOnMainThread(task => {
            Debug.Log("Username changed");
        });
        usernameConfirmationText.text = "Username updated successfully.";
    }

}
