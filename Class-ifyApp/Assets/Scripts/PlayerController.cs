using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

namespace Com.CS.Classify
{
    public class PlayerController : MonoBehaviourPun
    {
        public Rigidbody2D rb;
        public float moveSpeed;
        float xInput;
        float yInput;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        private AudioListener audioListener;
        private Camera mainCamera;
        private bool movementEnabled;
        private FirebaseAuth auth;
        private FirebaseUser user;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;
        private GameNetworkManager gameNetworkManager;
        private bool isHost = false;

        // Initialize local player instance if this is an instance of the local player
        void Awake() {
            movementEnabled = true;
            audioListener = GetComponent<AudioListener>();

            if (photonView.IsMine)
            {
                PlayerController.LocalPlayerInstance = this.gameObject;
            }

            gameNetworkManager = FindObjectOfType<GameNetworkManager>();
            if (gameNetworkManager == null)
            {
                Debug.LogError("Error: cannot find room notification manager script.");
            }
        }

        // Check if this is the local player's instance, if so have camera follow them, and attatch camera to player nametag canvas
        async void Start()
        {
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

            rb = GetComponent<Rigidbody2D>();

            if (photonView.IsMine)
            {
                SimpleCameraController cameraFollow = Camera.main.GetComponent<SimpleCameraController>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTarget(transform);
                }
            }

            DocumentSnapshot snapshot = await gameNetworkManager.GetRoomDataAsync();
            string host = snapshot.GetValue<string>("Host");

            Debug.Log("Host is: " + host + " and user email is: " + user.Email);

            if (host.Equals(""))
            {
                Debug.LogError("Error: host not set for this room.");
            }
            else if (host.Equals(user.Email))
            {
                isHost = true;
            }

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                _uiGo.SendMessage("SetHostStatus", isHost, SendMessageOptions.RequireReceiver);
                if (photonView.IsMine)
                {
                    _uiGo.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }

        public void setMovementEnabled(bool movementEnabled) {
            this.movementEnabled = movementEnabled;
        }

        // Check if this is local player's instance, if so enable movement, otherwise do nothing
        void Update()
        {
            if ((photonView.IsMine == false && PhotonNetwork.IsConnected == true) || !movementEnabled)
            {
                return;
            }

            GetInput();

            rb.velocity = new Vector2(xInput, yInput).normalized * moveSpeed;
        }

        void GetInput()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
        }
    }
}
