using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

namespace Com.CS.Classify
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Public fields

        public Button joinRoomButton;
        public Button createRoomButton;
        public TMP_InputField roomCode;
        public TextMeshProUGUI errorMessage;
        public GameObject playerPrefab;
        public LinearCongruentialGenerator codeGenerationLogic;

        #endregion

        #region Private Fields
        private FirebaseFirestore db;
        private FirebaseAuth auth;
        private FirebaseUser user;
        private string username;
        string gameVersion = "1";
        string roomCodeText;
        // private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region MonoBehaviour CallBacks

        // Called during early initialization, connects to master server, sets up listeners for join and create room buttons
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

            Connect();

            if (joinRoomButton != null)
            {
                joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
            }

             if (createRoomButton != null)
            {
                createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            }
        }

        // Called when script is loaded, logs errors
        async void Start()
        {
            if (codeGenerationLogic == null)
            {
                Debug.LogError("Error: codeGenerationLogic is not assigned in the Inspector.");
            }

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

        #endregion


        #region Methods

        // Connect to master server
        public void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        /// When join room button is clicked, join to room, otherwise throw error
        private void OnJoinRoomButtonClicked()
        {
            roomCodeText = roomCode.text;
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRoom(roomCode.text);
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot join a room.");
            }
        }

        /// When create room button is clicked, create a new room, otherwise throw error
        private async void OnCreateRoomButtonClicked()
        {
            roomCodeText = roomCode.text;
            bool randomCodeFlag = false;
            if (PhotonNetwork.IsConnected)
            {   
                if (roomCode.text == "")
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
                    //TODO change to reflect user input when creating room
                    InitRoomData(16);
                    CreateRoom(16);
                }
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot create a room.");
            }
        }

        // Check if a room exists in Firestore
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


        // Initialize or re-initialize room data from Firestore
        private void InitRoomData(int maxPlayers)
        {
            DocumentReference docRef = db.Collection("room").Document(roomCodeText);
            Dictionary<string, object> room = new Dictionary<string, object>
            {
                { "Host", user.Email },
                { "MaxPlayers", maxPlayers },
            };
            docRef.SetAsync(room).ContinueWithOnMainThread(task => {
                Debug.Log("Initialized room data in Firestore");
            });

            Debug.Log("Created room with host " + user.Email);
        }

        #endregion

        #region Custom Callbacks

        // Create a room with specified parameters
        private void CreateRoom(int maxPlayers)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
            PhotonNetwork.CreateRoom(roomCodeText, roomOptions);

            Debug.Log("(Re-)Created room with player limit of " + maxPlayers + ".");
        }

        private async void RejoinRoom()
        {
            DocumentSnapshot snapshot = await GetRoomDataAsync();
            int maxPlayers = snapshot.TryGetValue("MaxPlayers", out int maxPlayersValue) ? maxPlayersValue : 16;

            CreateRoom(maxPlayers);   
        }

        private async Task<DocumentSnapshot> GetRoomDataAsync()
        {
            DocumentReference docRef = db.Collection("room").Document(roomCodeText);
            
            try
            {
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                
                if (snapshot.Exists)
                {
                    return snapshot;
                }
                else
                {
                    Debug.LogError("Document does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error: Failed to check for room document: " + ex.Message);
                return null;
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


        // Failed to create room due to room code already existing
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

        // Join a room
        private void JoinRoom()
        {
            DataHolderMainMenu.Instance.UpdateSavedCode(roomCodeText);
            Debug.Log("Successfully joined room " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.NickName = username;
            PhotonNetwork.LoadLevel("RoomScene");
        }

        // Fail to join a room because it does not exist
        private void FailJoinRoom()
        {
            errorMessage.text = "room does not exist";
            Debug.Log("Unable to connect to room.");
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // Called on join room success
        public override void OnJoinedRoom()
        {
            JoinRoom();
        }

        // Called on join room failed
        public override async void OnJoinRoomFailed(short returnCode, string message)
        {
            bool exists = await DoesRoomExistAsync();
            if (exists)
            {
                RejoinRoom();
            }
            else
            {
                FailJoinRoom();
            }
        }

        // Called on create room success
        public override void OnCreatedRoom()
        {
        }

        // Called on create room failed
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            FailCreateRoom(false);
        }

        // Called on connect to master successful
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Launcher: OnConnectedToMaster() was called by PUN");
        }

        // Called on disconnected from master
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion

    }
}