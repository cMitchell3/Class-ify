using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Com.CS.Classify
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public Button leaveRoomButton;

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
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
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
            SceneManager.LoadScene(0);
        }

        // When another player enters the room, log information, instantiate them if local
        public override void OnPlayerEnteredRoom(Player other)
        {
            // Not seen if you're player joining
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }

            if (other.IsLocal)
            {
                Debug.LogFormat("Instantiating player for {0}", other.NickName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            }
        }

        // When another player leaves the room, log information
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }
        }

        #endregion
    }
}