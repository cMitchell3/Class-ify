using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Com.CS.Classify
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Public fields

        public Button joinRoomButton;
        public Button createRoomButton;
        //public InputField roomJoinCode;

        #endregion

        #region Private Fields
        
        string tempRoomJoinCode = "A5B3";

        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
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

            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        void Start()
        {
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

        /// When join room button is clicked, connect to room (temporary implementation for testing)
        private void OnJoinRoomButtonClicked()
        // NOTE: This will give an error right now because create room automatically joins the room but we do not have a scene set up yet where I can send the user once they join
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRoom(tempRoomJoinCode); // TODO update to include actual room code input field
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot join a room.");
            }
        }

        /// When create room button is clicked, create a new room (temporary implementation for testing)
        private void OnCreateRoomButtonClicked()
        {
            if (PhotonNetwork.IsConnected)
            {

                RoomOptions roomOptions = new RoomOptions { MaxPlayers = 16 }; 
                PhotonNetwork.CreateRoom(tempRoomJoinCode, roomOptions); // TODO update to include actual room code input field
            }
            else
            {
                Debug.LogWarning("Not connected to Photon server. Cannot create a room.");
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // Join room successful
        public override void OnJoinedRoom()
        {
            Debug.Log("Successfully joined a room: " + PhotonNetwork.CurrentRoom.Name);
        }

        // Join room failed, create new room
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