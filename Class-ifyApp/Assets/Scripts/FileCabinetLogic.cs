using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FileCabinetLogic : MonoBehaviour
{
    public GameObject fileCabinetObject;
 //   public GameObject playerController;


    public void ActivateFileCabinet()
    {
        fileCabinetObject.SetActive(!fileCabinetObject.activeSelf);
 //       playerController.GetComponent<PlayerController>().enabled = false;
    }

    public void Back()
    {
        fileCabinetObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
 //       playerController.GetComponent<PlayerController>().enabled = true;
    }
}
