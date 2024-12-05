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
    // private FirebaseAuth auth;
    // private FirebaseUser user;
    private string userEmail;

    private async void Start()
    {
        // Initialize Firebase
        // db = FirebaseFirestore.DefaultInstance;
        // auth = FirebaseAuth.DefaultInstance;
        // user = auth.CurrentUser;

        // Check if TextMeshPro component is assigned
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();

        // Load currency amount from Firestore and display it
        currencyAmount = await FirestoreManager.Instance.GetUserCurrency(userEmail);
        UpdateText();

        FirestoreManager.Instance.ListenToUserCollection(userEmail, OnCurrencyChanged);
        // LoadCurrencyFromDatabase();
    }

    private async void OnCurrencyChanged(int newCoinValue)
    {
        this.currencyAmount = newCoinValue;
        UpdateText();
    }

    // Load currency amount from Firestore
    // private async void LoadCurrencyFromDatabase()
    // {
    //     if (user == null)
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
        // currencyAmount += amount;
        // UpdateText();

        // Optionally, update Firestore with the new currency amount if needed
        FirestoreManager.Instance.UpdateUserCurrency(userEmail, amount);
    }

    // public void AddNumber(string userEmail, int amount)
    // {
    //     currencyAmount += amount;
    //     UpdateText();

    //     // Optionally, update Firestore with the new currency amount if needed
    //     FirestoreManager.Instance.UpdateUserCurrency(userEmail, amount);
    // }

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