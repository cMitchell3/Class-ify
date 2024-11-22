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

    public async Task<string> GetRoomHostEmail(string roomCode)
    {
        string hostEmail = "";
        DocumentReference docRef = db.Collection("room").Document(roomCode);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            hostEmail = snapshot.GetValue<string>("Host");
        }
        else
        {
            Debug.LogError("Room document: " + roomCode + " not found.");
        }

        return hostEmail;
    }

    public async void UpdateUserLoginRewardInfo(string email, UserLoginRewardInfo userLoginRewardInfo)
    {
        DocumentReference docRef = db.Collection("user").Document(email);

        DateTime lastUpdated = userLoginRewardInfo.GetLastUpdated();
        int streakNumber = userLoginRewardInfo.GetStreakNumber();
        bool isClaimed = userLoginRewardInfo.GetIsClaimed();

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "lastUpdated", lastUpdated },
            { "streakNumber", streakNumber },
            { "isClaimed", isClaimed },
        };

        await docRef.UpdateAsync(updates);
        Debug.Log($"User login rewards updated in Firestore: lastUpdated:{lastUpdated} streakNumber:{streakNumber} isClaimed:{isClaimed}");
    }

    public async void UpdateUserCurrency(string email, int amount)
    {
        DocumentReference docRef = db.Collection("user").Document(email);
        int currentCoins = await GetUserCurrency(email);
        int updateCoins = currentCoins + amount;

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "coins", updateCoins }
        };

        await docRef.UpdateAsync(updates);
        Debug.Log($"Currency updated in Firestore: {updateCoins} coins for {email}");
    }

    public async Task UploadFileToFirestore(string filePath, string roomCode)
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

        try
        {
            await docRef.SetAsync(fileData);

            Debug.Log("File uploaded successfully with ID: " + fileId);
            
            AddFileReferenceToRoom(fileId, roomCode);
        }
        catch (Exception)
        {
            throw;
        }
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

    private async Task<int> GetUserCurrency(string email)
    {
        int coins = 0;
        DocumentReference docRef = db.Collection("user").Document(email);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            coins = snapshot.GetValue<int>("coins");  
        }
        else
        {
            Debug.LogWarning("Error reading coins field from user " + email);
        }

        return coins;
    }

    public async Task<UserLoginRewardInfo> GetLoginRewardInfo(string email)
    {
        UserLoginRewardInfo userLoginRewardInfo = null;
        DocumentReference docRef = db.Collection("user").Document(email);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            try
            {
                DateTime lastUpdated = snapshot.GetValue<DateTime>("lastUpdated").ToLocalTime();
                int streakNumber = snapshot.GetValue<int>("streakNumber");  
                bool isClaimed = snapshot.GetValue<bool>("isClaimed");
                userLoginRewardInfo = new UserLoginRewardInfo(lastUpdated, streakNumber, isClaimed);
            }
            catch (Exception)
            {
                userLoginRewardInfo = null;
            }
        }
        else
        {
            Debug.LogWarning("Error reading coins field from user " + email);
        }

        return userLoginRewardInfo;
    }

    public async Task<FileInfo> GetFileInfo(string fileId)
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

    public async void UpdateUserInventory(string email, int cosmeticId)
    {
        string[] inventoryItems = await GetUserInventory(email);
        string[] updatedItems = new string[inventoryItems.Length + 1];

        string cosmeticIdStr = cosmeticId.ToString();

        for (int i = 0; i < updatedItems.Length; i++)
        {
            if (updatedItems[i] == cosmeticIdStr) //user already has this cosmetic
            {
                Debug.LogWarning("User already has this cosmetic.");
                return;
            }

            if (i < inventoryItems.Length)
            {
                updatedItems[i] = inventoryItems[i];
            }
            else
            {
                updatedItems[i] = cosmeticIdStr;
            }
        }

        string inventoryContent = string.Join(",", updatedItems);

        DocumentReference docRef = db.Collection("user").Document(email);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "inventory", inventoryContent }
        };

        await docRef.UpdateAsync(updates);
        Debug.Log($"Inventory updated in Firestore for {email}.");
    }

    public async Task<string[]> GetUserInventory(string email)
    {
        string[] inventoryItems = new string[0];

        DocumentReference docRef = db.Collection("user").Document(email);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists && snapshot.ContainsField("inventory"))
        {
            string inventoryContent = snapshot.GetValue<string>("inventory");
            inventoryItems = inventoryContent.Split(',');
        }
        else
        {
            Debug.LogWarning("No inventory field found for user.");
        }

        return inventoryItems;
    }
}
