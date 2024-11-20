using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

namespace Com.CS.Classify
{
    public class CreateRoomMenuLogic : MonoBehaviourPunCallbacks
    {
        // Inputs and Fields
        public TMP_InputField userCountInput;
        public TMP_InputField roomCode;
        public TextMeshProUGUI errorMessage;
        public Button plusButton;
        public Button minusButton;
        public Button createRoomButton;
        public Button backButton;
        public LinearCongruentialGenerator codeGenerationLogic;

        // Values
        private int currentUsers = 1;
        private string roomCodeText;
        private const int minUsers = 1;
        private const int maxUsers = 16;
        // private Launcher launcher;
        private FirebaseFirestore db;
        private FirebaseAuth auth;
        private FirebaseUser user;
        private string username;
        
        void Awake()
        {
            if (createRoomButton != null)
            {
                createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            }
            
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackButtonClicked);
            }

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

        async void Start()
        {
            if (codeGenerationLogic == null)
            {
                Debug.LogError("Error: codeGenerationLogic is not assigned in the Inspector.");
            }

            userCountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            userCountInput.text = currentUsers.ToString();

            plusButton.onClick.AddListener(IncrementUsers);
            minusButton.onClick.AddListener(DecrementUsers);

            userCountInput.onEndEdit.AddListener(OnInputValueChanged);

            DocumentSnapshot snapshot = await GetUserDataAsync();
            if (snapshot != null)
            {
                this.username = snapshot.TryGetValue("username", out string usernameOut) ? usernameOut : "";
            }
            else
            {
                this.username = user.Email.Split('@')[0];
            }
        }

        private async Task<DocumentSnapshot> GetUserDataAsync()
        {
            DocumentReference docRef = db.Collection("user").Document(user.Email);
            
            try
            {
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                
                if (snapshot.Exists)
                {
                    return snapshot;
                }
                else
                {
                    Debug.LogWarning("User ocument does not exist, using email prefix.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error: Failed to check for user document: " + ex.Message);
                return null;
            }
        }

        void OnBackButtonClicked()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        async void OnCreateRoomButtonClicked()
        {
            roomCodeText = roomCode.text;
            bool randomCodeFlag = false;
            if (PhotonNetwork.IsConnected)
            {   
                if (roomCodeText == "")
                {
                    codeGenerationLogic.RandomizeSeed();
                    roomCodeText = codeGenerationLogic.Next().ToString();
                    randomCodeFlag = true;
                }

                bool exists = await DoesRoomExistAsync();
                if (exists)
                {
                    FailCreateRoom(randomCodeFlag);
                }
                else
                {
                    int maxPlayers = currentUsers;
                    InitRoomData(maxPlayers);
                    CreateRoom(maxPlayers);
                }
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot create a room.");
            }
        }

        private void CreateRoom(int maxPlayers)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
            PhotonNetwork.CreateRoom(roomCodeText, roomOptions);

            Debug.Log("(Re-)Created room with player limit of " + maxPlayers + ".");
        }

        private void JoinRoom()
        {
            DataHolderMainMenu.Instance.UpdateSavedCode(roomCodeText);
            Debug.Log("Successfully joined room " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.NickName = username;
            PhotonNetwork.LoadLevel("RoomScene");
        }

        public override void OnJoinedRoom()
        {
            JoinRoom();

            AddUserToArray(roomCodeText);
        }

        // Adds the creator of the room to the active user array in Firestore
        public void AddUserToArray(string roomCode)
        {
            // Reference the Firestore document for the specific room
            DocumentReference roomRef = db.Collection("room").Document(roomCode);

            // Use ArrayUnion to add the user email to the users array
            roomRef.UpdateAsync("ActiveUsers", FieldValue.ArrayUnion(PhotonNetwork.NickName))
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log($"User {PhotonNetwork.NickName} added to room {roomCode}.");
                    }
                    else
                    {
                        Debug.LogError($"Error adding user {PhotonNetwork.NickName} to room {roomCode}: {task.Exception}");
                    }
                });
        }

         private void FailCreateRoom(bool randomCodeFlag)
        {
            if (randomCodeFlag)
            {
                roomCodeText = "";
                OnCreateRoomButtonClicked();
            }
            else
            {
                errorMessage.text = "room code already in use";
                Debug.Log("Unable to create room, room with same code already exists.");
            }
        }

        private void InitRoomData(int maxPlayers)
        {
            DocumentReference docRef = db.Collection("room").Document(roomCodeText);
            Dictionary<string, object> room = new Dictionary<string, object>
            {
                { "Host", user.Email },
                { "MaxPlayers", maxPlayers },
                { "ActiveUsers", new string[0] },
            };
            docRef.SetAsync(room).ContinueWithOnMainThread(task => {
                Debug.Log("Initialized room data in Firestore");
            });

            Debug.Log("Created room with host " + user.Email);
        }

        private async Task<bool> DoesRoomExistAsync()
        {
            DocumentReference docRef = db.Collection("room").Document(roomCodeText);

            try
            {
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                return snapshot.Exists;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error: Failed to check for room document: " + ex.Message);
                return false;
            }
        }

        void IncrementUsers()
        {
            if (currentUsers < maxUsers)
            {
                currentUsers++;
                UpdateInputField();
            }
        }

        void DecrementUsers()
        {
            if (currentUsers > minUsers)
            {
                currentUsers--;
                UpdateInputField();
            }
        }

        void UpdateInputField()
        {
            userCountInput.text = currentUsers.ToString();
        }

        void OnInputValueChanged(string value)
        {
            if (int.TryParse(value, out int parsedValue))
            {
                currentUsers = Mathf.Clamp(parsedValue, minUsers, maxUsers);
            }
            else
            {
                userCountInput.text = currentUsers.ToString();
            }

            UpdateInputField();
        }
    }
}
