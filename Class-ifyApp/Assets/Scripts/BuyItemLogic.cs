using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.UI;


public class BuyItemLogic : MonoBehaviour
{

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;


    public TextMeshProUGUI textTwo; // "Coins: [number]" in TextMeshPro format
    public Button optionOne;
    public Button optionTwo;
    public Button optionThree;
    public Button optionFour;
    public Button optionFive;

    private string inventoryContent = "";

    private CurrencyDisplayController currencyDisplay;

    private void Start()
    {

        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        // Find and assign the CurrencyDisplayController in the scene
        currencyDisplay = FindObjectOfType<CurrencyDisplayController>();

        LoadInventoryFromDatabase();

        string[] inventoryContents = inventoryContent.Split(",");
        for (int i = 0; i < inventoryContents.Length; i++) {
            if (inventoryContents[i].Equals("1")) {
                IncreaseButtonOpacity(optionOne.GetComponentInChildren<TextMeshProUGUI>());
            }
            if (inventoryContents[i].Equals("2")) {
                IncreaseButtonOpacity(optionTwo.GetComponentInChildren<TextMeshProUGUI>());
            }
            if (inventoryContents[i].Equals("3")) {
                IncreaseButtonOpacity(optionThree.GetComponentInChildren<TextMeshProUGUI>());
            }
            if (inventoryContents[i].Equals("4")) {
                IncreaseButtonOpacity(optionFour.GetComponentInChildren<TextMeshProUGUI>());
            }
            if (inventoryContents[i].Equals("5")) {
                IncreaseButtonOpacity(optionFive.GetComponentInChildren<TextMeshProUGUI>());
            }
        }
        
    }

    public void SubtractCoins(TextMeshProUGUI buttonText)
    {
        // Extract numbers from the button text and textTwo
        int coinsOne = ExtractNumberFromTextOne(buttonText.text);
        int coinsTwo = ExtractNumberFromTextTwo(textTwo.text);


        // Subtract coinsOne from coinsTwo
        int newCoinsTwo = coinsTwo - coinsOne;

        if ((coinsTwo - coinsOne) >= 0) {
            currencyDisplay.AddNumber(-1 * coinsOne);

            // Update textTwo with the new value
            textTwo.text = $"Coins: {newCoinsTwo}";

            // Increase the opacity of the chosen button
            IncreaseButtonOpacity(buttonText);

            Button button = buttonText.transform.parent.GetComponent<Button>();

            if (!inventoryContent.Equals("")) {
                inventoryContent += ",";
            }

            if (button.GetInstanceID() == optionOne.GetInstanceID()) {
                inventoryContent += "1";
            }

            if (button.GetInstanceID() == optionTwo.GetInstanceID()) {
                inventoryContent += "2";
            }

            if (button.GetInstanceID() == optionThree.GetInstanceID()) {
                inventoryContent += "3";
            }

            if (button.GetInstanceID() == optionFour.GetInstanceID()) {
                inventoryContent += "4";
            }

            if (button.GetInstanceID() == optionFive.GetInstanceID()) {
                inventoryContent += "5";
            }

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

    public async void UpdateInventoryInDatabase()
    {
        if (user == null)
        {
            Debug.LogWarning("No authenticated user found.");
            return;
        }

        DocumentReference docRef = db.Collection("user").Document(user.Email);
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "inventory", inventoryContent }
        };

        await docRef.UpdateAsync(updates);
        Debug.Log($"Inventory updated in Firestore: {inventoryContent}");
    }

    private async void LoadInventoryFromDatabase()
    {
        if (user == null)
        {
            Debug.LogWarning("No authenticated user found.");
            return;
        }

        DocumentReference docRef = db.Collection("user").Document(user.Email);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists && snapshot.ContainsField("inventory"))
        {
            inventoryContent = snapshot.GetValue<string>("inventory");
            Debug.Log($"Inventory loaded: {inventoryContent}");
            ApplyInventoryToButtons();
        }
        else
        {
            Debug.LogWarning("No inventory field found for user.");
        }

    }

    private void ApplyInventoryToButtons()
    {
        // Split inventory string and update button states based on contents
        string[] inventoryContents = inventoryContent.Split(',');

        foreach (string item in inventoryContents)
        {
            if (item == "1")
            {
                IncreaseButtonOpacity(optionOne.GetComponentInChildren<TextMeshProUGUI>());
            }
            else if (item == "2")
            {
                IncreaseButtonOpacity(optionTwo.GetComponentInChildren<TextMeshProUGUI>());
            }
            else if (item == "3")
            {
                IncreaseButtonOpacity(optionThree.GetComponentInChildren<TextMeshProUGUI>());
            }
        }
    }

    public string getInventoryContent() {
        return inventoryContent;
    }
}