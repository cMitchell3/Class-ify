using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteraction : MonoBehaviourPun
{
    public Button thumbsUpButton;
    public Image thumbsUpIcon;

    private void Start()
    {
        if (photonView.IsMine)
        {
            thumbsUpButton.onClick.AddListener(() => GiveThumbsUp());
        }
        else
        {
            thumbsUpButton.gameObject.SetActive(false);
        }

        thumbsUpIcon.gameObject.SetActive(false); // Start with icon hidden
    }

    public void GiveThumbsUp()
    {
        if (!photonView.IsMine) return;

        // Send RPC to the clicked player’s PhotonView ID
        photonView.RPC("ReceiveThumbsUp", RpcTarget.Others, photonView.ViewID);
    }

    [PunRPC]
    private void ReceiveThumbsUp(int senderViewID)
    {
        // Show thumbs up icon
        StartCoroutine(DisplayThumbsUp());
        
        // Optionally: Handle coin increment on your backend here for the receiver
    }

    private IEnumerator DisplayThumbsUp()
    {
        thumbsUpIcon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // Show for 2 seconds
        thumbsUpIcon.gameObject.SetActive(false);
    }
}
