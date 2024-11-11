using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }
    public FirebaseFirestore db { get; private set; }
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
        
        InitializeFirestore();
    }

    void InitializeFirestore()
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

    public void UploadFileToFirestore(string filePath)
    {
        string fileId = Guid.NewGuid().ToString();
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string uploadUser = FirebaseAuthManager.Instance.GetUserEmail();
        string base64Content = FileConverter.ConvertFileToBase64(filePath);
        string extension = Path.GetExtension(filePath);

        DocumentReference docRef = db.Collection("file").Document(fileId);
        Dictionary<string, object> fileData = new Dictionary<string, object>
            {
                { "FileName", fileName },
                { "UploadUser",  uploadUser },
                { "Content", base64Content },
                { "Type", extension },
            };

        docRef.SetAsync(fileData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("File uploaded successfully with ID: " + fileId);
            }
            else
            {
                Debug.LogError("Error uploading file, file is likely too large: " + task.Exception);
            }
        });
    }

    public void DownloadFileFromFirestore(string fileId)
    {
        Debug.Log("Download file from firestore");
        DocumentReference docRef = db.Collection("file").Document(fileId);
        docRef.GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string fileName = snapshot.GetValue<string>("FileName");

                    // Commented out for now but will be used in future features, gives user who uploaded file
                    // string uploadUser = snapshot.GetValue<string>("UploadUser");

                    string base64Content = snapshot.GetValue<string>("Content");
                    byte[] fileBytes = Convert.FromBase64String(base64Content);
                    string extension = snapshot.GetValue<string>("Type");

                    ExportFile.SaveFile(fileName, fileBytes, extension);

                    Debug.Log("File downloaded and saved successfully.");
                }
                else
                {
                    Debug.LogError("File document not found.");
                }
            }
            else
            {
                Debug.LogError("Error downloading file: " + task.Exception);
            }
        });
    }

    public void ListenToRoomCollection(string roomId, Action<List<string>> onFilesChanged)
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

                    onFilesChanged?.Invoke(currentFileIds);
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
