using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThumbsUpInteraction : MonoBehaviourPun
{
    public GameObject thumbsUpButtonPrefab;
    private Button thumbsUpButton;
    public Sprite thumbsUpSprite;  // Set the thumbs-up sprite in the Inspector

    private void Start()
    {
        // Only allow interaction if the local player is not the owner
        if (!photonView.IsMine)
        {
            SetUpThumbsUpButton();
        }
    }

    private void SetUpThumbsUpButton()
    {
        // Instantiate the button and parent it to the player
        thumbsUpButton = Instantiate(thumbsUpButtonPrefab, transform).GetComponent<Button>();
        thumbsUpButton.image.sprite = thumbsUpSprite;
        thumbsUpButton.onClick.AddListener(() => SendThumbsUpToPlayer());
        
        // Set the button to be active for other players but inactive for the owner
        thumbsUpButton.gameObject.SetActive(!photonView.IsMine);
    }

    private void SendThumbsUpToPlayer()
    {
        // Send RPC call to give a thumbs-up to the player with photonView ID
        photonView.RPC("DisplayThumbsUp", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    private void DisplayThumbsUp(int targetViewID)
    {
        if (photonView.ViewID == targetViewID)
        {
            StartCoroutine(DisplayThumbsUpIcon());
        }
    }

    private IEnumerator DisplayThumbsUpIcon()
    {
        // Show thumbs up for a limited time
        thumbsUpButton.image.enabled = true;
        yield return new WaitForSeconds(2.0f);
        thumbsUpButton.image.enabled = false;
    }
}
