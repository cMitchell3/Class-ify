using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class RoomCodeDisplayController : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    private string joinCode = "";

    // On startup get room code and initialize text
    public void Start() {
        if (DataHolderMainMenu.Instance != null)
        {
            joinCode = DataHolderMainMenu.Instance.savedCode;

            Debug.Log("The saved room code is: " + joinCode);
        }
        else
        {
            Debug.LogError("DataHolderMainMenu instance is null! Make sure it's present in the main menu scene.");
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
       DocumentReference docRef = db.Collection("room").Document(joinCode);
       Dictionary<string, object> room = new Dictionary<string, object>
       {
          { "Host", "Shelby" },
          { "Name", "CS 307" },
       };
        docRef.SetAsync(room).ContinueWithOnMainThread(task =>
        {
          Debug.Log("Added data to the " + joinCode + " document in the room collection");
        });

        CollectionReference roomRef = db.Collection("room");
        roomRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log(String.Format("Room: {0}", document.Id));
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Debug.Log(String.Format("Host: {0}", documentDictionary["Host"]));
                Debug.Log(String.Format("Name: {0}", documentDictionary["Name"]));
            }

            Debug.Log("Read all data from the users collection.");
        });
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        UpdateText();
    }

    // Updates the currency UI number
    private void UpdateText()
    {
        tmpText.text = "Join Code: " + joinCode;
    }
}
