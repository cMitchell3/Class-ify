using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerInteractionManager : MonoBehaviourPun
{
    // Reference to the thumbs up icon
    public GameObject thumbsUpIcon;

    private void Start()
    {
        // Ensure the thumbs-up icon is initially inactive
        if (thumbsUpIcon != null)
        {
            thumbsUpIcon.SetActive(false);
        }
        else
        {
            Debug.LogError("ThumbsUpIcon is not assigned in the inspector.");
        }
    }

    private void OnMouseDown()
    {
        // When this player is clicked, trigger the thumbs-up icon to appear for both players
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
        Debug.Log("Player Clicked.");
        if (thumbsUpIcon != null)
        {
            Debug.Log("Starting Coroutine.");
            StartCoroutine(ShowIconTemporarily());
        }
    }

    private IEnumerator ShowIconTemporarily()
    {
        // Activate the thumbs-up icon
        Debug.Log("Setting Active.");
        thumbsUpIcon.SetActive(true);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Deactivate the thumbs-up icon
        thumbsUpIcon.SetActive(false);
    }
}
