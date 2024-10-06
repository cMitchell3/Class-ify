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
            LocalPlayerInstance = gameObject;
        }
        else {
            Destroy(audioListener);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            if (photonView.IsMine)
            {
                // If this player is the local player, set the Main Camera to follow it
                mainCamera.transform.localPosition = new Vector3(0, 0, -10);
            }
        }
        else
        {
            Debug.LogError("Main Camera not found in the scene.");
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
