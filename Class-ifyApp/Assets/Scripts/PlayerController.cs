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

    // Initialize local player instance if this is an instance of the local player
    void Awake() {
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

        // Enable local player's camera and audio listener
        if (photonView.IsMine)
        {
            Camera mainCamera = GetComponentInChildren<Camera>(true);
            if (mainCamera != null)
            {
                mainCamera.enabled = true; 

                AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = true;
                }
            }
        }
        else
        {
            // Locally disable the camera and audio listener for other players' prefabs
            Camera otherCamera = GetComponentInChildren<Camera>(true);
            if (otherCamera != null)
            {
                otherCamera.enabled = false;
                AudioListener otherAudioListener = otherCamera.GetComponent<AudioListener>();
                if (otherAudioListener != null)
                {
                    otherAudioListener.enabled = false;
                }
            }
        }
    }

    // Check if this is local player's instance, if so enable movement, otherwise do nothing
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
