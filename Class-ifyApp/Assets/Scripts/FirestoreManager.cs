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

    //TODO attatch to room
    public void UploadFileToFirestore(string filePath, string roomCode)
    {
        string fileId = Guid.NewGuid().ToString();
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string uploadUser = FirebaseAuthManager.Instance.GetUserEmail();
        string base64Content = FileConverter.ConvertFileToBase64(filePath);
        string extension = Path.GetExtension(filePath).TrimStart('.');

        DocumentReference docRef = db.Collection("file").Document(fileId);
        Dictionary<string, object> fileData = new Dictionary<string, object>
            {
                { "FileName", fileName },
                { "UploadUser",  uploadUser },
                { "Content", base64Content },
                { "Extension", extension },
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

        AddFileReferenceToRoom(fileId, roomCode);
    }

    private void AddFileReferenceToRoom(string fileId, string roomCode)
    {
        DocumentReference fileDocRef = db.Collection("file").Document(fileId);
        DocumentReference roomDocRef = db.Collection("room").Document(roomCode);

        roomDocRef.UpdateAsync("files", FieldValue.ArrayUnion(fileDocRef)).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("File reference added to room: " + roomCode);
            }
            else
            {
                Debug.LogError("Error adding file reference to room: " + task.Exception);
            }
        });
    }

    public void DeleteFileFromFirestore(string fileId, string roomCode)
    {
        DocumentReference fileDocRef = db.Collection("file").Document(fileId);

        fileDocRef.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("File removed successfully from 'file' collection with ID: " + fileId);
            }
            else
            {
                Debug.LogError("Error removing file from 'file' collection: " + task.Exception);
            }
        });

        RemoveFileReferenceFromRoom(fileId, roomCode);
    }

    private void RemoveFileReferenceFromRoom(string fileId, string roomCode)
    {
        DocumentReference roomDocRef = db.Collection("room").Document(roomCode);
        DocumentReference fileDocRef = db.Collection("file").Document(fileId);

        roomDocRef.UpdateAsync("files", FieldValue.ArrayRemove(fileDocRef)).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("File reference removed from room's 'files' array: " + roomCode);
            }
            else
            {
                Debug.LogError("Error removing file reference from room: " + task.Exception);
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

    public async Task<FileInfo> ReadFileInfo(string fileId)
    {
        FileInfo fileInfo = null;
        DocumentReference docRef = db.Collection("file").Document(fileId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            string fileName = snapshot.GetValue<string>("FileName");
            string extension = snapshot.GetValue<string>("Extension");
            string uploadUser = snapshot.GetValue<string>("UploadUser");
            string base64Content = snapshot.GetValue<string>("Content");
            byte[] content = Convert.FromBase64String(base64Content);

            fileInfo = new FileInfo(fileId, fileName, extension, uploadUser, content);
        }
        else
        {
            Debug.LogError("File document: " + fileId + " not found.");
        }

        return fileInfo;
    }
}
