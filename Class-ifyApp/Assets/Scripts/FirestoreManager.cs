using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }
    public FirebaseFirestore db { get; private set; }
    private List<string> currentDocIds = new List<string>();
    private HashSet<string> previousFileIds = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeFirebase();
    }

    void InitializeFirebase()
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
    }

    public void ListenToRoomCollection(string roomId, Action<List<string>, List<string>> onFilesChanged)
    {
        Debug.Log("Listening for room collection file changes");
        DocumentReference roomRef = db.Collection("room").Document(roomId);

        roomRef.Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                var roomData = snapshot.ToDictionary();

                if (roomData.ContainsKey("files"))
                {
                    var fileReferences = roomData["files"] as IEnumerable<object>;
                    List<string> currentFileIds = new List<string>();

                    foreach (var fileRef in fileReferences)
                    {
                        if (fileRef is DocumentReference docRef)
                        {
                            currentFileIds.Add(docRef.Id);
                        }
                        else
                        {
                            Debug.LogWarning("Encountered a non-reference entry in 'files' array.");
                        }
                    }

                    List<string> addedFileIds = currentFileIds.Except(previousFileIds).ToList();
                    List<string> removedFileIds = previousFileIds.Except(currentFileIds).ToList();

                    onFilesChanged?.Invoke(addedFileIds, removedFileIds);
                    previousFileIds = new HashSet<string>(currentFileIds);
                }
                else
                {
                    Debug.LogWarning("No 'files' array found in the room document.");
                }
            }
            else
            {
                Debug.LogWarning("Room document does not exist.");
            }
        });
    }
}
