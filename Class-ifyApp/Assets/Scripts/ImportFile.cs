using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using TMPro;
using System;

public class ImportFile : MonoBehaviour
{
    public Button importButton;
    public TextMeshProUGUI errorMessage;
    public TextMeshProUGUI roomCodeDisplay;
    public CurrencyDisplayController currencyDisplayController;
    private string userEmail;
    private static int importCoins = 10;
    private Color errorRed;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        if (importButton != null)
        {
            importButton.onClick.AddListener(OnImportButtonClicked);
        }   

        Color color;
        ColorUtility.TryParseHtmlString("#C70D0D", out color);
        this.errorRed = color;

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();
    }

    public async void OnImportButtonClicked()
    {
        errorMessage.text = "";
        
        string filePath = OpenFileDialog();
        if (!filePath.Equals(""))
        {
            string roomCode = roomCodeDisplay.text.Split(" ")[2];
            try
            {
                errorMessage.color = Color.white;
                errorMessage.text = "Uploading...";
                await FirestoreManager.Instance.UploadFileToFirestore(filePath, roomCode);
                FirestoreManager.Instance.UpdateUserCurrency(userEmail, importCoins);
                currencyDisplayController.AddNumber(importCoins, false);
                errorMessage.color = errorRed;
                errorMessage.text = "";
            }
            catch (Exception)
            {
                errorMessage.color = errorRed;
                errorMessage.text = "Error uploading file, is your file too large (>1MB)?";
            }
        }
    }

    public string OpenFileDialog()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select a file", "", "", false);

        string filePath = "";

        if (paths.Length > 0)
        {
            filePath = paths[0];
            Debug.Log("File selected: " + filePath);
        }
        else
        {
            Debug.Log("No file selected.");
        }

        return filePath;
    }
}
