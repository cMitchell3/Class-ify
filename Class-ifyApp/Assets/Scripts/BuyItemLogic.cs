using UnityEngine;
using UnityEngine.UI;

public class BuyItemLogic : MonoBehaviour
{
    public Text textOne; // "[number] Coins"
    public Text textTwo; // "Coins: [number]"

    public void SubtractCoins()
    {
        // Extract numbers from the two text fields
        int coinsOne = ExtractNumberFromTextOne(textOne.text);
        int coinsTwo = ExtractNumberFromTextTwo(textTwo.text);

        // Subtract coinsOne from coinsTwo
        int newCoinsTwo = coinsTwo - coinsOne;

        // Update textTwo with the new value
        textTwo.text = $"Coins: {newCoinsTwo}";
    }

    private int ExtractNumberFromTextOne(string text)
    {
        // "[number] Coins" -> Extract [number]
        string[] parts = text.Split(' ');
        int.TryParse(parts[0], out int result);
        return result;
    }

    private int ExtractNumberFromTextTwo(string text)
    {
        // "Coins: [number]" -> Extract [number]
        string[] parts = text.Split(' ');
        int.TryParse(parts[1], out int result);
        return result;
    }
}