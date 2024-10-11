using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

namespace Com.CS.Classify
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Public fields

        public Button joinRoomButton;
        public Button createRoomButton;
        public TMP_InputField roomCode;
        public GameObject playerPrefab;
        public LinearCongruentialGenerator codeGenerationLogic;
        // public DataHolderMainMenu dataHolder;

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        // Called during early initialization, sets up listeners for join and create room buttons
        void Awake()
        {
            if (joinRoomButton != null)
            {
                joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
            }

             if (createRoomButton != null)
            {
                createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            }
        }

        // Called when script is loaded, connects to master server
        void Start()
        {
            if (codeGenerationLogic == null)
            {
                Debug.LogError("codeGenerationLogic is not assigned in the Inspector.");
            }

            // if (dataHolder != null)
            // {
            //     Debug.LogError("dataHolder is not assigned in the Inspector.");
            // }

            Connect();
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
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Joining room " + roomCode.text);
                PhotonNetwork.JoinRoom(roomCode.text);
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot join a room.");
            }
        }

        /// When create room button is clicked, create a new room, otherwise throw error
        private void OnCreateRoomButtonClicked()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (roomCode.text == "") {
                    roomCode.text = codeGenerationLogic.Next().ToString();
                }

                DataHolderMainMenu.Instance.UpdateSavedCode(roomCode.text);
                RoomOptions roomOptions = new RoomOptions { MaxPlayers = 16 }; 
                PhotonNetwork.CreateRoom(roomCode.text, roomOptions);
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot create a room.");
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // After joining a room, load classroom scene
        public override void OnJoinedRoom()
        {
            Debug.Log("Successfully joined room " + PhotonNetwork.CurrentRoom.Name);

            PhotonNetwork.LoadLevel("RoomScene");
        }

        // Join room failed
         public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Unable to connect to room.");
        }

        // Connect to master successful
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Launcher: OnConnectedToMaster() was called by PUN");
        }

        // Disconnected from master
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion

    }
}