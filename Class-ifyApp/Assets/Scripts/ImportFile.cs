using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using TMPro;

public class ImportFile : MonoBehaviour
{
    public Button importButton;
    public TextMeshProUGUI roomCodeDisplay;
    private string userEmail;
    private static int importCoins = 10;

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

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();
    }

    public void OnImportButtonClicked()
    {
        string filePath = OpenFileDialog();
        if (!filePath.Equals(""))
        {
            string roomCode = roomCodeDisplay.text.Split(" ")[2];
            FirestoreManager.Instance.UploadFileToFirestore(filePath, roomCode);
            FirestoreManager.Instance.UpdateUserCurrency(userEmail, importCoins);
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
