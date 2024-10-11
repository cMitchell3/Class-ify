using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsLogic : MonoBehaviour
{
    public GameObject settingsMenu;
    public PauseLogic pauseLogic;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void PauseToSettingsButton()
    {
        pauseLogic.pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsMenu.SetActive(false);
        pauseLogic.pauseMenu.SetActive(true);
    }
}
