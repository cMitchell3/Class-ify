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

    // Initialize local player instance if this is an instance of the local player
    void Awake() {
        movementEnabled = true;
        audioListener = GetComponent<AudioListener>();

        if (photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
    }

    // Check if this is the local player's instance, if so enable main camera and audio listener, otherwise disable them
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Make the camera a child of this player
        GameObject mainCamera = GameObject.Find("MainCamera");
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 1, -10);
            mainCamera.transform.localRotation = Quaternion.identity;
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
