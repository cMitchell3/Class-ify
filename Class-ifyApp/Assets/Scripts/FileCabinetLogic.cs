using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.CS.Classify
{
    public class FileCabinetLogic : MonoBehaviour
    {
        public GameObject fileCabinetObject;
    public GameObject playerController;


        public void ActivateFileCabinet()
        {
            fileCabinetObject.SetActive(!fileCabinetObject.activeSelf);
            EnablePlayerMovement(false);
        }

        public void Back()
        {
            fileCabinetObject.SetActive(false);
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
}
