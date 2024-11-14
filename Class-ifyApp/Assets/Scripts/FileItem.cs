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
        
    }

    private void OnDeleteButtonClicked()
    {
        
    }
}
