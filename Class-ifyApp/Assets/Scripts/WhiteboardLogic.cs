using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class WhiteboardLogic : MonoBehaviour
{
    public GameObject whiteboardObject;

    public void ActivateWhiteboard()
    {
        Debug.Log("Activated whiteboard, disabled player controller");
        whiteboardObject.SetActive(!whiteboardObject.activeSelf);
        EnablePlayerMovement(false);
    }

    public void Back()
    {
        whiteboardObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EnablePlayerMovement(true);
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
