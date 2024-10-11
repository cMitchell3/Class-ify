using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsLogic : MonoBehaviour
{   

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
}
