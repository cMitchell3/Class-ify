using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ThumbsUpInteraction : MonoBehaviourPunCallbacks
{
    public Button thumbsUpButton;           // Button for giving a thumbs up
    public GameObject thumbsUpUI;           // UI element for thumbs up action (thumbs-up button and background)
    public Image thumbsUpIcon;              // Icon that appears briefly to show the thumbs-up action
 //   public TextMeshProUGUI feedbackText;    // Optional: text to confirm interaction
    private PhotonView photonView;

    private const byte ThumbsUpEventCode = 1;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        if (thumbsUpButton != null)
        {
            thumbsUpButton.onClick.AddListener(OnThumbsUpClicked);
        }

        // Ensure thumbs-up UI starts hidden
        thumbsUpUI.SetActive(false);

        // Subscribe to Photon events
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    // Called when another player clicks on this player’s button
    public void ShowThumbsUpUI()
    {
        thumbsUpUI.SetActive(true);
    }

    private void OnThumbsUpClicked()
    {
        // Hide thumbs-up UI after click
        thumbsUpUI.SetActive(false);

        // Send thumbs-up event to other players
        object[] content = { photonView.ViewID, 15 };  // The ViewID identifies the player, and 15 is the coin amount.
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(ThumbsUpEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ThumbsUpEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int targetViewID = (int)data[0];
            int coinAmount = (int)data[1];

            if (photonView.ViewID == targetViewID)
            {
                StartCoroutine(DisplayThumbsUpIcon());

                // Add coins to this player’s account if this is the targeted player
                // Assuming CurrencyDisplayController handles updating the currency for the local player
                CurrencyDisplayController currencyDisplay = FindObjectOfType<CurrencyDisplayController>();
                if (currencyDisplay != null)
                {
                    currencyDisplay.AddNumber(coinAmount);
 /*                   if (feedbackText != null)
                    {
                        feedbackText.text = "+15 Coins!";
                        StartCoroutine(HideFeedbackText());
                    }
 */               }
            }
        }
    }

    private IEnumerator DisplayThumbsUpIcon()
    {
        thumbsUpIcon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);  // Display thumbs-up icon for 2 seconds
        thumbsUpIcon.gameObject.SetActive(false);
    }

/*    private IEnumerator HideFeedbackText()
    {
        yield return new WaitForSeconds(1.5f);
        feedbackText.text = "";
    }
*/}
