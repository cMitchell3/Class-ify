using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseLogic : MonoBehaviour
{
    public GameObject pauseMenu;


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
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
