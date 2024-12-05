using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;

public class CurrencyDisplayController : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    private int currencyAmount = 0;
    
    private string userEmail;

    private async void Start()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();

        currencyAmount = await FirestoreManager.Instance.GetUserCurrency(userEmail);
        UpdateText();

        FirestoreManager.Instance.ListenToUserCollection(userEmail, OnCurrencyChanged);
    }

    private async void OnCurrencyChanged(int newCoinValue)
    {
        this.currencyAmount = newCoinValue;
        UpdateText();
    }

    // Add/Subtract a certain amount to currency
    public void AddNumber(int amount)
    {
        FirestoreManager.Instance.UpdateUserCurrency(userEmail, amount);
    }

    // Update the currency UI number
    private void UpdateText()
    {
        tmpText.text = "Coins: " + currencyAmount.ToString();
    }

    public int getCurrencyAmount() {
        return currencyAmount;
    }

}