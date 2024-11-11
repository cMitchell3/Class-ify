using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FileItem : MonoBehaviour
{
    public TextMeshProUGUI fileName;
    public Button downloadButton;
    public Button deleteButton;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        if (downloadButton != null)
        {
            downloadButton.onClick.AddListener(OnDownloadButtonClicked);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

    }

    private void OnDownloadButtonClicked()
    {
        Debug.Log("Download button clicked");
        FirestoreManager.Instance.DownloadFileFromFirestore("9048b6e0-6427-4aab-9a25-295629e298ce");
    }

    private void OnDeleteButtonClicked()
    {
        
    }
}
