using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WhiteboardLogic : MonoBehaviour
{
    public GameObject whiteboardObject;
    // public GameObject playerController;


    public void ActivateWhiteboard()
    {
        Debug.Log("Activated whiteboard");
        whiteboardObject.SetActive(!whiteboardObject.activeSelf);
        // playerController.GetComponent<PlayerController>().enabled = false;
    }

    public void Back()
    {
        whiteboardObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        // playerController.GetComponent<PlayerController>().enabled = true;
    }
}
