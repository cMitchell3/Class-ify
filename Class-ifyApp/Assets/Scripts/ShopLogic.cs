using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Auth;

public class ShopLogic : MonoBehaviour
{
    private CurrencyDisplayController currencyDisplay;
    private BuyItemLogic buyItem;

    private void Start()
    {
        // Find and assign the CurrencyDisplayController and BuyItemLogic in the scene
        currencyDisplay = FindObjectOfType<CurrencyDisplayController>();
        buyItem = FindObjectOfType<BuyItemLogic>();
    }

    public async void BackButton()
    {
        // Check if both controllers are available
        if (currencyDisplay != null && buyItem != null)
        {
            var userDocument = FirebaseFirestore.DefaultInstance.Collection("user").Document(FirebaseAuth.DefaultInstance.CurrentUser.Email);
            
            // Prepare updates for both currency and inventory
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "coins", currencyDisplay.getCurrencyAmount() },
                { "inventory", buyItem.getInventoryContent() }
            };

            // Update both fields at once to Firestore
            await userDocument.UpdateAsync(updates);
            Debug.Log("Currency and inventory updated in Firestore");
        }

        // Load the main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }
}