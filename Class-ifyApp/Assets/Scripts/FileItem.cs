using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FileItem : MonoBehaviour
{
    public TextMeshProUGUI fileTextDisplay;
    public Button downloadButton;
    public Button deleteButton;
    private FileCabinetContent fileCabinetContent;
    private FileInfo fileInfo;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        GameObject parentObject = this.transform.parent.gameObject;
        fileCabinetContent = parentObject.GetComponent<FileCabinetContent>();

        if (downloadButton != null)
        {
            downloadButton.onClick.AddListener(OnDownloadButtonClicked);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

    }

    public void SetFileInfo(FileInfo fileInfo)
    {
        this.fileInfo = fileInfo;
        this.fileTextDisplay.text = $"{fileInfo.GetFileName()}.{fileInfo.GetExtension()}";
    }

    private void OnDownloadButtonClicked()
    {
        ExportFile.SaveFile(fileInfo.GetFileName(), fileInfo.GetExtension(), fileInfo.GetContent(), fileInfo.GetUploadUser());
    }

    private void OnDeleteButtonClicked()
    {
        fileCabinetContent.DeleteFile(fileInfo.GetFileId());   
    }
}
