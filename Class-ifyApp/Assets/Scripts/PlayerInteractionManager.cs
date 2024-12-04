using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInteractionManager : MonoBehaviourPun
{
    public GameObject interactionButtonPrefab; 
    public GameObject thumbsUpIconPrefab;      
    public Vector3 screenOffset = new Vector3(0f, 2f, 0f); // Offset for UI above players

    private Transform playerTransform;       
    private GameObject interactionButton;   
    private GameObject thumbsUpIconInstance;
    private GameObject thumbsUpUI;             
    private Button noThumbsUpButton;           
    private Button thumbsUpYesButton;
    private string targetPlayerNickName;

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
            thumbsUpUI.SetActive(false); 

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
            }
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

        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(true);
        }

        targetPlayerNickName = photonView.Owner.NickName;
    }

    private void OnNoThumbsUpButtonClicked()
    {
        Debug.Log("Thumbs-up interaction canceled.");
        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(false); 
        }
    }

    private void OnThumbsUpYesButtonClicked()
    {
        if (thumbsUpUI != null)
        {
            thumbsUpUI.SetActive(false);
        }

        // Call RPC to show thumbs-up icon and reward coins
        if (!string.IsNullOrEmpty(targetPlayerNickName) && PhotonNetwork.IsConnected)
        {
            photonView.RPC("ShowThumbsUpIcon", RpcTarget.All);
            photonView.RPC("RewardCoins", RpcTarget.AllBuffered, targetPlayerNickName, 10);
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
        thumbsUpIconInstance.SetActive(true); 

        yield return new WaitForSeconds(2); 

        thumbsUpIconInstance.SetActive(false); 
    }

    [PunRPC]
    public void RewardCoins(string targetUserId, int amount)
    {
        // Find the CurrencyDisplayController in the scene
        Debug.Log($"Player Name: {PhotonNetwork.NickName}");
        Debug.Log($"Search Name: {targetUserId}");
        if (PhotonNetwork.LocalPlayer.NickName == targetUserId)
        {
           // Find the CurrencyDisplayController for the local player
            CurrencyDisplayController currencyController = FindObjectOfType<CurrencyDisplayController>();

            if (currencyController != null)
            {
                currencyController.AddNumber(amount, true); // Add coins
                Debug.Log($"Added {amount} coins to {photonView.Owner.NickName}");
            }
            else
            {
                Debug.LogWarning("CurrencyDisplayController not found for the local player.");
            }
        }
    }
}
