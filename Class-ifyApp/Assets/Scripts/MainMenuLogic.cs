using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    public Button openLoginRewardsButton;
    public GameObject loginRewardsPanel;

    void Start()
    {
        if (openLoginRewardsButton != null)
        {
            openLoginRewardsButton.onClick.AddListener(OnOpenLoginRewardsButtonClicked);
        }
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ShopButton()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void CharacterButton() {
        SceneManager.LoadScene("InventoryMenu");
    }

    private void OnOpenLoginRewardsButtonClicked()
    {
        loginRewardsPanel.SetActive(true);
    }
}
