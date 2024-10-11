using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyDisplayController : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    private int currencyAmount = 0;

    // On startup initialize text
    public void Start() {
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        UpdateText();
    }

    // Add/Subtract a certain amount to currency
    public void AddNumber(int amount)
    {
        currencyAmount += amount;
        UpdateText();
    }

    // Updates the currency UI number
    private void UpdateText()
    {
        tmpText.text = "Coins: " + currencyAmount.ToString();
    }
}
