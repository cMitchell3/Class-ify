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
    public class GameNetworkManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public KickPlayerHandler kickPlayerHandler;
        public Button leaveRoomButton;
        public Button testKickButton;
        public TMP_InputField testKickPlayerName;
        public TextMeshProUGUI roomCodeDisplay;
        public Transform kickMenu;
        public GameObject playerListElement; 
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

            if (testKickButton != null)
            {
                testKickButton.onClick.AddListener(OnTestKickButtonClicked);
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

            PhotonNetwork.EnableCloseConnection = true;
        }

        // Called when script is loaded, instantiates player
        public void Start() {
            InstantiatePlayer();          
        }

        #endregion

        #region Methods

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StartCoroutine(FetchHostAndPopulateMenu());
            }
        }

        private IEnumerator FetchHostAndPopulateMenu()
        {
            // Start fetching room data asynchronously
            Task<DocumentSnapshot> fetchHostTask = GetRoomDataAsync();
            yield return new WaitUntil(() => fetchHostTask.IsCompleted);

            if (fetchHostTask.Result == null)
            {
                Debug.LogError("Failed to retrieve room data.");
                yield break;
            }

            // Get the host name
            Task<string> hostUsernameTask = FetchHostUsernameAsync();
            yield return new WaitUntil(() => hostUsernameTask.IsCompleted);
            string hostUsername = hostUsernameTask.Result;

            bool isCurrentPlayerHost = PhotonNetwork.NickName == hostUsername;
            //bool isCurrentPlayerHost = "gongoozler" == hostUsername;
            // Debug.Log($"Player found with username: {PhotonNetwork.NickName}.");
            // Debug.Log($"Host found with username: {hostUsername}.");
            // Debug.Log($"Host and player username comparasion resulted in {isCurrentPlayerHost}.");



            //bool isCurrentPlayerHost = String.Equals("gongoozler", hostUsername);

            // Call PopulateKickMenu with the host info
            StartCoroutine(PopulateKickMenu(isCurrentPlayerHost));
        }

        IEnumerator PopulateKickMenu(bool isCurrentPlayerHost)
        {
            // Toggle KickMenu visibility
            kickMenu.gameObject.SetActive(!kickMenu.gameObject.activeSelf);

            if (!kickMenu.gameObject.activeSelf) yield break; // If just closing the menu, skip populating

            // Clear any existing elements in the KickMenu
            foreach (Transform child in kickMenu)
            {
                Destroy(child.gameObject);
            }

            // Get active users asynchronously
            Task<string[]> activeUsersTask = GetActiveUsers();
            yield return new WaitUntil(() => activeUsersTask.IsCompleted);

            if (activeUsersTask.Result == null)
            {
                Debug.LogError("Failed to fetch active users.");
                yield break;
            }

            string[] playersInRoom = activeUsersTask.Result;
            foreach (string playerName in playersInRoom)
            {
                // Instantiate the playerListElement prefab under kickMenu
                GameObject newElement = Instantiate(playerListElement, kickMenu);

                // Assign player name to PlayerName text field
                TextMeshProUGUI playerNameText = newElement.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
                playerNameText.text = playerName;

                // Set up KickButton functionality
                Button kickButton = newElement.transform.Find("KickButton").GetComponent<Button>();
                kickButton.onClick.AddListener(() => KickPlayer(playerName));

                // Enable kick button only if the current player is the host
                //kickButton.interactable = isCurrentPlayerHost;
                kickButton.gameObject.SetActive(isCurrentPlayerHost);
            }
        }

        // Fetches the host's username from the "user" collection based on email.
        private async Task<string> FetchHostUsernameAsync()
        {
            // Retrieve room data to get host email
            DocumentSnapshot roomSnapshot = await GetRoomDataAsync();
                    if (roomSnapshot == null || !roomSnapshot.TryGetValue("Host", out string hostEmail))
            {
                Debug.LogError("Error: Host email not found in room data.");
                return null;
            }

            // Use host email to look up the username in the "user" collection
            DocumentReference userDocRef = db.Collection("user").Document(hostEmail);
            DocumentSnapshot userSnapshot = await userDocRef.GetSnapshotAsync();

            if (userSnapshot != null && userSnapshot.TryGetValue("username", out string hostUsername))
            {
                //Debug.LogError($"Success: Host found with username: {hostUsername}.");
                return hostUsername;
            }
            else
            {
                Debug.LogError($"Error: Could not retrieve username for host with email {hostEmail}.");
                return null;
            }

        }

        // Method to kick a player by name
        void KickPlayer(string playerName)
        {
            kickPlayerHandler.RequestKickPlayer(playerName);
        }

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

        private void OnTestKickButtonClicked()
        {
            kickPlayerHandler.RequestKickPlayer(testKickPlayerName.text);
        }

        public Player GetPlayerByUsername(string playerName)
        {
            Debug.Log("Finding player with username" + playerName);
            Player targetPlayer = null;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == playerName)
                {
                    targetPlayer = player;
                    break;
                }
            }

            if (targetPlayer == null)
            {
                Debug.LogError($"Player with nickname {playerName} not found.");
            }
            
            return targetPlayer;
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
            string username = other.NickName;
            Debug.LogFormat("OnPlayerEnteredRoom() " + username);
            FindObjectOfType<RoomNotificationManager>().ShowPlayerJoined(username);

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
            string username = other.NickName;
            Debug.LogFormat("OnPlayerLeftRoom() {0}", username);
            FindObjectOfType<RoomNotificationManager>().ShowPlayerLeft(username);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }
        }

        #endregion
    }
}