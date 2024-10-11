using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ShopButton()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void CharacterButton()
    {
        SceneManager.LoadScene("InventoryMenu");
    }
}
