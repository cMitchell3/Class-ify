using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInteractionManager : MonoBehaviourPun
{
    public GameObject interactionButtonPrefab; // Prefab for the button
    public GameObject thumbsUpIconPrefab;      // Prefab for thumbs-up icon
    public Vector3 screenOffset = new Vector3(0f, 2f, 0f); // Offset for UI above players

    private Transform playerTransform;       // Reference to this player's transform
    private GameObject interactionButton;   // Instance of the interaction button
    private GameObject thumbsUpIconInstance; // Instance of the thumbs-up icon
    private PhotonView targetPhotonView;     // Store the target PhotonView for confirmation
    private GameObject thumbsUpUI;             // Reference to the confirmation UI canvas
    private Button noThumbsUpButton;           // Reference to the "No" button
    private Button thumbsUpYesButton;          // Reference to the "Yes" button


    void Start()
{
    playerTransform = this.transform;

    // Find and set up the interaction button
    GameObject canvas = GameObject.Find("Canvas");
    if (canvas != null && interactionButtonPrefab != null)
    {
        interactionButton = Instantiate(interactionButtonPrefab, canvas.transform);
        interactionButton.GetComponent<Button>().onClick.AddListener(OnInteractionButtonClicked);

        // Search for ThumbsUpUI within the same canvas
        thumbsUpUI = canvas.transform.Find("ThumbsUpUI")?.gameObject;

        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(false); // Initially hide the confirmation UI

            noThumbsUpButton = thumbsUpUI.transform.Find("NoThumbsUpButton")?.GetComponent<Button>();
            thumbsUpYesButton = thumbsUpUI.transform.Find("ThumbsUpYesButton")?.GetComponent<Button>();

            if (noThumbsUpButton != null)
            {
                noThumbsUpButton.onClick.AddListener(OnNoThumbsUpButtonClicked);
                thumbsUpYesButton = noThumbsUpButton.transform.Find("ThumbsUpYesButton")?.GetComponent<Button>();

                if (thumbsUpYesButton != null)
                {
                    thumbsUpYesButton.onClick.AddListener(OnThumbsUpYesButtonClicked);
                }
            else
            {
                Debug.LogError("ThumbsUpYesButton not found in ThumbsUpUI.");
            }
            }
            else
            {
                Debug.LogError("NoThumbsUpButton not found in ThumbsUpUI.");
            }

            
        }
        else
        {
            Debug.LogError("ThumbsUpUI canvas not found within the main Canvas.");
        }
    }
    else
    {
        Debug.LogError("Main Canvas or InteractionButtonPrefab not assigned.");
    }

    // Find and set up the thumbs-up icon
    if (canvas != null && thumbsUpIconPrefab != null)
    {
        thumbsUpIconInstance = Instantiate(thumbsUpIconPrefab, canvas.transform);
        thumbsUpIconInstance.SetActive(false); // Initially inactive
    }
    else
    {
        Debug.LogError("ThumbsUpIconPrefab not assigned.");
    }

    if (photonView != null)
    {
        Debug.Log("PhotonView found on this GameObject!");
    }
    else
    {
        Debug.LogError("PhotonView is missing on this GameObject!");
    }
}

    void LateUpdate()
    {
        if (interactionButton != null && playerTransform != null)
        {
            // Convert player position to screen space and apply offset
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position) + screenOffset;
            interactionButton.transform.position = screenPoint;
        }

        if (thumbsUpIconInstance != null && playerTransform != null)
        {
            // Same logic for the thumbs-up icon
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position) + screenOffset;
            thumbsUpIconInstance.transform.position = screenPoint;
        }
    }

    private void OnInteractionButtonClicked()
    {
        Debug.Log($"Interaction button clicked for player: {photonView.Owner.NickName}");

        // Store the PhotonView of the clicked player
        targetPhotonView = photonView;

        // Activate confirmation UI for the clicking player
        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(true);
        }
    }

    private void OnNoThumbsUpButtonClicked()
    {
        Debug.Log("Thumbs-up interaction canceled.");
        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(false); // Hide the confirmation UI
        }
    }

    private void OnThumbsUpYesButtonClicked()
    {
        Debug.Log("Thumbs-up interaction confirmed.");

        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(false); // Hide the confirmation UI
        }

        // Call RPC to show thumbs-up icon and reward coins
        if (targetPhotonView != null && PhotonNetwork.IsConnected)
        {
            targetPhotonView.RPC("ShowThumbsUpIcon", RpcTarget.All);
            targetPhotonView.RPC("RewardCoins", RpcTarget.AllBuffered, photonView.Owner.NickName, 10);
        }
        else
        {
            Debug.LogError("Target PhotonView is null or not connected to Photon.");
        }
    }

    [PunRPC]
    public void ShowThumbsUpIcon()
    {
        if (thumbsUpIconInstance != null)
        {
            StartCoroutine(ShowIconTemporarily());
        }
    }

    private System.Collections.IEnumerator ShowIconTemporarily()
    {
        thumbsUpIconInstance.SetActive(true); // Activate thumbs-up icon

        yield return new WaitForSeconds(2); // Wait for 2 seconds

        thumbsUpIconInstance.SetActive(false); // Deactivate thumbs-up icon
    }

    [PunRPC]
    public void RewardCoins(string targetUserId, int amount)
    {
        // Find the CurrencyDisplayController in the scene
        Debug.Log($"Adding coins for player: {photonView.Owner.NickName}");
        if (PhotonNetwork.NickName == targetUserId)
        {
           // Find the CurrencyDisplayController for the local player
            CurrencyDisplayController currencyController = FindObjectOfType<CurrencyDisplayController>();

            if (currencyController != null)
            {
                currencyController.AddNumber(amount); // Add coins
                Debug.Log($"Added {amount} coins to {photonView.Owner.NickName}");
            }
            else
            {
                Debug.LogWarning("CurrencyDisplayController not found for the local player.");
            }
        }
//        else
//        {
//            Debug.log($"RewardCoins ignored for player: {PhotonNetwork.LocalPlayer.Nickname}");
//        }  
    }
}
