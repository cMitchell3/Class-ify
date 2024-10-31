using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopLogic : MonoBehaviour
{
    private CurrencyDisplayController currencyDisplay;

    private void Start()
    {
        // Find and assign the CurrencyDisplayController in the scene
        currencyDisplay = FindObjectOfType<CurrencyDisplayController>();
    }

    public void BackButton()
    {
        // Write back the currency amount to Firestore before navigating
        if (currencyDisplay != null)
        {
            currencyDisplay.UpdateCurrencyInDatabase();
        }
        
        // Load the main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }
}