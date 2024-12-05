using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseLogic : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject chatBox;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ActivatePauseMenu();
    }

    void ActivatePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (pauseMenu.activeSelf == false)
            {
                chatBox.SetActive(true);
            }
            else
            {
                chatBox.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        chatBox.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
