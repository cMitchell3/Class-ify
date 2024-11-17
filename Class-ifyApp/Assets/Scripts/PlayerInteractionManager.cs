using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerInteractionManager : MonoBehaviourPun
{
    public GameObject thumbsUpIconPrefab; // Prefab for thumbs-up icon (assigned in inspector)
    public Vector3 screenOffset = new Vector3(0f, 2f, 0f); // Offset for the button above the player

    private Transform playerTransform; // Reference to this player's transform
    private GameObject thumbsUpIconInstance; // Instance of thumbs-up icon for this player

    void Start()
    {
        playerTransform = this.transform;

        // Instantiate thumbs-up icon and set its parent to the canvas
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null && thumbsUpIconPrefab != null)
        {
            thumbsUpIconInstance = Instantiate(thumbsUpIconPrefab, canvas.transform);
            thumbsUpIconInstance.SetActive(false); // Initially inactive
        }
        else
        {
            Debug.LogError("Canvas or ThumbsUpIconPrefab not assigned.");
        }
    }

    void LateUpdate()
    {
        if (thumbsUpIconInstance != null && playerTransform != null)
        {
            // Convert player position to screen space and apply the offset
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position) + screenOffset;
            thumbsUpIconInstance.transform.position = screenPoint;
        }
    }

    public void OnMouseDown()
    {
        // When a player is clicked, show thumbs-up icon on both players
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("ShowThumbsUpIcon", RpcTarget.All);
        }
        else
        {
            Debug.LogError("Not connected to Photon.");
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

    private IEnumerator ShowIconTemporarily()
    {
        thumbsUpIconInstance.SetActive(true); // Activate the thumbs-up icon

        yield return new WaitForSeconds(2); // Wait for 2 seconds

        thumbsUpIconInstance.SetActive(false); // Deactivate the thumbs-up icon
    }
}
