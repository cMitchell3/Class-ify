using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FileCabinetLogic : MonoBehaviour
{
    public GameObject fileCabinetMenu;
    public GameObject playerController;


    public void ActivateFileCabinetMenu()
    {

        fileCabinetMenu.SetActive(!fileCabinetMenu.activeSelf);
        playerController.GetComponent<PlayerController>().enabled = false;
    }

    public void Back()
    {
        fileCabinetMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        playerController.GetComponent<PlayerController>().enabled = true;
    }
}
