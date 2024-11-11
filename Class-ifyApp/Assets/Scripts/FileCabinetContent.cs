using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileCabinetContent : MonoBehaviour
{
    public TextMeshProUGUI roomCodeDisplay;
    public GameObject filePrefab;
    private List<FileItem> files;
    [SerializeField]
    private GridLayoutGroup fileGroup;
    public List<string> currentFileIds;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        string roomCode = roomCodeDisplay.text.Split(" ")[2];
        FirestoreManager.Instance.ListenToRoomCollection(roomCode, OnFilesChanged);
    }

    private void OnFilesChanged(List<string> currentFileIds)
    {
        //TODO update UI
        Debug.Log("Files have been changed");
        this.currentFileIds = currentFileIds;

        foreach (var fileId in currentFileIds)
        {
            Debug.Log("File: " + fileId);
        }

        UpdateFileContent();
    }

    private void UpdateFileContent()
    {

    }
}
