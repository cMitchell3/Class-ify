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

    void Awake() {
        audioListener = GetComponent<AudioListener>();

        if (photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
        else {
            Destroy(audioListener);
        }

        // DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Check if this is the local player's instance
        if (photonView.IsMine)
        {
            // Ensure the local player's camera is active
            Camera mainCamera = GetComponentInChildren<Camera>(true);
            if (mainCamera != null)
            {
                mainCamera.enabled = true;  // Enable the camera for the local player
                AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = true;  // Ensure audio listener is enabled for local player
                }
            }
        }
        else
        {
            // Disable the camera and audio listener for remote players
            Camera otherCamera = GetComponentInChildren<Camera>(true);
            if (otherCamera != null)
            {
                otherCamera.enabled = false;  // Disable the camera for remote players
                AudioListener otherAudioListener = otherCamera.GetComponent<AudioListener>();
                if (otherAudioListener != null)
                {
                    otherAudioListener.enabled = false;  // Disable the audio listener for remote players
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
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
