using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FileCabinetLogic : MonoBehaviour
{
    public GameObject fileCabinetObject;
    public GameObject playerController;
    public TextMeshProUGUI errorMessage;
    public GameObject chatBox;

    public void ActivateFileCabinet()
    {
        fileCabinetObject.SetActive(!fileCabinetObject.activeSelf);
        EnablePlayerMovement(false);
        chatBox.SetActive(false);
    }

    public void Back()
    {
        fileCabinetObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EnablePlayerMovement(true);
        errorMessage.text = "";
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
