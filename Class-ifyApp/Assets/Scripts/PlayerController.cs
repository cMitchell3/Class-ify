using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;

    // Initialize local player instance if this is an instance of the local player
    void Awake() {
        movementEnabled = true;
        audioListener = GetComponent<AudioListener>();

        if (photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
    }

    // Check if this is the local player's instance, if so have camera follow them, and attatch camera to player nametag canvas
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (photonView.IsMine)
        {
            SimpleCameraController cameraFollow = Camera.main.GetComponent<SimpleCameraController>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(transform);
            }

        //     Canvas canvas = GetComponentInChildren<Canvas>();

        //     if (canvas != null)
        //     {
        //         Camera mainCamera = Camera.main;

        //         if (mainCamera != null)
        //         {
        //             canvas.renderMode = RenderMode.WorldSpace;
        //             canvas.worldCamera = mainCamera;
        //         }
        //         else
        //         {
        //             Debug.LogError("Main camera not found. Make sure there is a camera tagged as MainCamera in the scene.");
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError("Canvas component not found on the name tag.");
        //     }

        }

         if (PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
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
