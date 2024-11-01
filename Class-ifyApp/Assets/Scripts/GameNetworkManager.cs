using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

using TMPro;

namespace Com.CS.Classify
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public Button leaveRoomButton;
        public TextMeshProUGUI roomCodeDisplay;
        private RoomNotificationManager roomNotificationManager;
        private FirebaseFirestore db;

        #endregion

        #region Public Methods

        // Player leaves room
        public void LeaveRoom()
        {
            Debug.Log("Leaving room");
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Called in early initialization phase, sets up button listeners
        void Awake()
        {
            if (leaveRoomButton != null)
            {
                leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
            }

            roomNotificationManager = FindObjectOfType<RoomNotificationManager>();
            if (roomNotificationManager == null)
            {
                Debug.LogError("Error: cannot find room notification manager script.");
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
        }

        // Called when script is loaded, instantiates player
        public void Start() {
            InstantiatePlayer();          
        }

        #endregion

        #region Methods

        // Instantiates the local player, throws an error if player is not in room or player prefab is not set up
        public void InstantiatePlayer() {
            if (!PhotonNetwork.InRoom)
            {
                Debug.LogError("Player is not in a room. Cannot instantiate player.");
                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else  {
                if (PlayerController.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);
                    GameObject playerInstance = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        private async Task<DocumentSnapshot> GetRoomDataAsync()
        {
            string roomCode = roomCodeDisplay.text.Split(" ")[2];
            DocumentReference docRef = db.Collection("room").Document(roomCode);
            
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

        private async Task<string[]> GetActiveUsers()
        {
            DocumentSnapshot snapshot = await GetRoomDataAsync();
            return snapshot.TryGetValue("ActiveUsers", out string[] activeUsers) ? activeUsers : new string[0];
        }

        /// When leave room button is clicked, leave room
        private void OnLeaveRoomButtonClicked()
        {
            LeaveRoom();
        }

        #endregion


        #region MonoBehaviourPunCallbacks Callbacks

        // After player leaves room, send them to main menu scene
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        // When another player enters the room, log information, instantiate them if local
        public override void OnPlayerEnteredRoom(Player other)
        {
            // Not seen if you're player joining
            Debug.LogFormat("OnPlayerEnteredRoom() " + other.NickName);
            FindObjectOfType<RoomNotificationManager>().ShowPlayerJoined(other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }

            if (other.IsLocal)
            {
                Debug.LogFormat("Instantiating player for {0}", other.NickName);
                GameObject playerInstance = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            }
        }

        // When another player leaves the room, log information
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
            FindObjectOfType<RoomNotificationManager>().ShowPlayerLeft(other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }
        }

        #endregion
    }
}