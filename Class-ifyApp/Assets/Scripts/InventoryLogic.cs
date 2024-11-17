using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryLogic : MonoBehaviour
{
    [SerializeField] private GameObject playerCosmeticsMenu;
    [SerializeField] private GameObject roomCosmeticsMenu;


    public void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void PlayerCosmeticsButton()
    {
        roomCosmeticsMenu.SetActive(false);
        playerCosmeticsMenu.SetActive(true);
    }

    public void RoomCosmeticsButton()
    {
        playerCosmeticsMenu.SetActive(false);
        roomCosmeticsMenu.SetActive(true);
    }
}
