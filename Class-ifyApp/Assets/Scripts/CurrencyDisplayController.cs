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

    // Firebase variables
    // private FirebaseFirestore db;
    private string userEmail;

    private async void Start()
    {
        // Initialize Firebase
        // db = FirebaseFirestore.DefaultInstance;
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();

        // Check if TextMeshPro component is assigned
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        // Load currency amount from Firestore and display it
        // LoadCurrencyFromDatabase();
        currencyAmount = await FirestoreManager.Instance.GetUserCurrency(userEmail);
        UpdateText();
    }

    // Load currency amount from Firestore
    // private async void LoadCurrencyFromDatabase()
    // {
    //     if (userEmail == null)
    //     {
    //         Debug.LogWarning("No authenticated user found.");
    //         return;
    //     }

    //     DocumentReference docRef = db.Collection("user").Document(userEmail);
    //     DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    //     if (snapshot.Exists && snapshot.ContainsField("coins"))
    //     {
    //         // Retrieve and update currency amount
    //         currencyAmount = snapshot.GetValue<int>("coins");
    //         UpdateText();
    //         Debug.Log($"Currency loaded: {currencyAmount} coins");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No coins field found for user.");
    //     }
    // }

    // Add/Subtract a certain amount to currency
    public void AddNumber(int amount)
    {
        currencyAmount += amount;
        UpdateText();

        // Optionally, update Firestore with the new currency amount if needed
        FirestoreManager.Instance.UpdateUserCurrency(userEmail, amount);
        // UpdateCurrencyInDatabase();
    }

    // Update the currency UI number
    private void UpdateText()
    {
        tmpText.text = "Coins: " + currencyAmount.ToString();
    }

    // Update currency amount in Firestore
    // public async void UpdateCurrencyInDatabase()
    // {
    //     if (user == null)
    //     {
    //         Debug.LogWarning("No authenticated user found.");
    //         return;
    //     }

    //     DocumentReference docRef = db.Collection("user").Document(userEmail);
    //     Dictionary<string, object> updates = new Dictionary<string, object>
    //     {
    //         { "coins", currencyAmount }
    //     };

    //     await docRef.UpdateAsync(updates);
    //     Debug.Log($"Currency updated in Firestore: {currencyAmount} coins");
    // }

    public int getCurrencyAmount() {
        return currencyAmount;
    }

}