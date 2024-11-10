using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileCabinetContent : MonoBehaviour
{
    public TextMeshProUGUI roomCodeDisplay;
    private List<FileItem> files;
    [SerializeField]
    private GridLayoutGroup fileGroup;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }
        
        Debug.Log("Starting file cabinet content");

        string roomCode = roomCodeDisplay.text.Split(" ")[2];
        FirestoreManager.Instance.ListenToRoomCollection(roomCode, OnFilesChanged);
        // FirestoreManager.Instance.ListenToRoom(roomCode);
    }

    private void OnFilesChanged(List<string> addedFileIds, List<string> removedFileIds)
    {
        //TODO update UI
        Debug.Log("Files have been changed");
        foreach (var fileId in addedFileIds)
        {
            Debug.Log("Added File: " + fileId);
        }

        foreach (var fileId in removedFileIds)
        {
            Debug.Log("Removed File with ID: " + fileId);
        }
    }
}
