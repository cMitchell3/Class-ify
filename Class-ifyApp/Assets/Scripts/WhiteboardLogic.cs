using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Com.CS.Classify;

public class WhiteboardLogic : MonoBehaviour
{
    public GameObject whiteboardObject;
    public Button lockButton;
    public GameNetworkManager gameNetworkManager;
    public GameObject chatBox;

    private async void Start()
    {
        if (gameNetworkManager != null)
        {
            // Fetch the host status asynchronously
            bool isHost = await gameNetworkManager.FetchHost();

            // Set the visibility of the lock button based on whether the player is the host
            lockButton.gameObject.SetActive(isHost);
        }
        else
        {
            Debug.LogError("GameNetworkManager reference is not set!");
        }
    }

    public void ActivateWhiteboard()
    {
        whiteboardObject.SetActive(!whiteboardObject.activeSelf);
        EnablePlayerMovement(false);
        chatBox.SetActive(false);
    }

    public void Back()
    {
        whiteboardObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EnablePlayerMovement(true);
        chatBox.SetActive(true);
    }

    private void EnablePlayerMovement(bool isEnabled)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        
        foreach (var player in players)
        {
            if (player.photonView.IsMine)
            {
                player.setMovementEnabled(isEnabled);
                break;
            }
        }
    }
}
