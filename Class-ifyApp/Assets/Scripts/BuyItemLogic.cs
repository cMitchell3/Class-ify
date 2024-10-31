using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinSubtraction : MonoBehaviour
{
    public TextMeshProUGUI textTwo; // "Coins: [number]" in TextMeshPro format
    public Button optionOne;
    public Button optionTwo;
    public Button optionThree;

    public void SubtractCoins(TextMeshProUGUI buttonText)
    {
        // Extract numbers from the button text and textTwo
        int coinsOne = ExtractNumberFromTextOne(buttonText.text);
        int coinsTwo = ExtractNumberFromTextTwo(textTwo.text);

        // Subtract coinsOne from coinsTwo
        int newCoinsTwo = coinsTwo - coinsOne;

        if (newCoinsTwo >= 0) {
            // Update textTwo with the new value
            textTwo.text = $"Coins: {newCoinsTwo}";

            // Increase the opacity of the chosen button
            IncreaseButtonOpacity(buttonText);
        }

        
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

    private void IncreaseButtonOpacity(TextMeshProUGUI buttonText)
    {
        // Get the Image component from the button's parent (the button itself)
        Image buttonImage = buttonText.transform.parent.GetComponent<Image>();
        Button button = buttonText.transform.parent.GetComponent<Button>();

        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = Mathf.Clamp(color.a + 0.8f, 0, 1); // Increase opacity by 20%, max of 1
            buttonImage.color = color;
        }

        if (buttonImage != null) {
            button.interactable = false;
        }
    }
}