using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerController : MonoBehaviourPun
{
    // Movement and Animation
    public Rigidbody2D rb;
    public float moveSpeed;
    float xInput;
    float yInput;
    public Animator animator;
    private Vector2 movement;
    private Vector2 lastMovement;
    private Vector2 blockedDirection;

    // Photon
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
        animator = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            SimpleCameraController cameraFollow = Camera.main.GetComponent<SimpleCameraController>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(transform);
            }
        }

         if (PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
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

        // Movement
        GetInput();

        // Animations
        Animate();
    }

    void FixedUpdate()
    {
        Vector2 adjustedMovement = movement;

        if (Vector2.Dot(movement, blockedDirection) < 0)
        {
            adjustedMovement -= Vector2.Dot(movement, blockedDirection) * blockedDirection;
        }

        rb.velocity = adjustedMovement * moveSpeed;
    }

    void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (xInput != 0)
        {
            yInput = 0;
        }
        else if (yInput != 0)
        {
            xInput = 0;
        }

        animator.SetFloat("xInput", xInput);
        animator.SetFloat("yInput", yInput);

        movement = new Vector2(xInput, yInput);
    }

    void Animate()
    {
        if (movement != Vector2.zero)
        {
            lastMovement = movement;
        }

        if (movement == Vector2.zero)
        {
            // Player is idle
            animator.SetBool("IsMoving", false);

            if (lastMovement.x > 0)
            {
                animator.SetInteger("LastDirection", 1);  // Right
            }
            else if (lastMovement.x < 0)
            {
                animator.SetInteger("LastDirection", 2);  // Left
            }
            else if (lastMovement.y > 0)
            {
                animator.SetInteger("LastDirection", 3);  // Up
            }
            else if (lastMovement.y < 0)
            {
                animator.SetInteger("LastDirection", 4);  // Down
            }
        }
        else
        {
            // Player is running
            animator.SetBool("IsMoving", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            blockedDirection = collision.contacts[0].normal;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            blockedDirection = Vector2.zero;
        }
    }
}
