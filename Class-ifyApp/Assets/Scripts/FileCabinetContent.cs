using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class FileCabinetContent : MonoBehaviour
{
    public TextMeshProUGUI roomCodeDisplay;
    public GameObject filePrefab;
    [SerializeField]
    private GridLayoutGroup fileGroup;
    private List<string> currentFileIds;
    private Dictionary<string, GameObject> fileItemInstances = new Dictionary<string, GameObject>();
    private string roomCode;

    void Start()
    {
        if (FirestoreManager.Instance == null || FirestoreManager.Instance.db == null)
        {
            Debug.LogError("Firestore or FirestoreManager instance is not initialized.");
        }

        roomCode = roomCodeDisplay.text.Split(" ")[2];
        currentFileIds = new List<string>();
        FirestoreManager.Instance.ListenToRoomCollection(roomCode, OnFilesChanged);
    }

    //TODO also remove file from db in room document and file collection
    public void DeleteFile(string fileId)
    {
        Debug.Log("Deleting file: " + fileId);
        if (fileItemInstances.TryGetValue(fileId, out GameObject fileInstance))
        {
            Destroy(fileInstance);
            fileItemInstances.Remove(fileId);
        }

        FirestoreManager.Instance.DeleteFileFromFirestore(fileId, roomCode);
    }

    private async void OnFilesChanged(List<string> outputFileIds)
    {
        var fileIdsToRemove = currentFileIds.Except(outputFileIds).ToList();
        foreach (var fileId in fileIdsToRemove)
        {
            if (fileItemInstances.TryGetValue(fileId, out GameObject fileInstance))
            {
                Destroy(fileInstance);
                fileItemInstances.Remove(fileId);
            }
        }

        var fileIdsToAdd = outputFileIds.Except(currentFileIds).ToList();
        foreach (var fileId in fileIdsToAdd)
        {
            GameObject newFileItem = Instantiate(filePrefab, fileGroup.transform);
            FileItem fileItemComponent = newFileItem.GetComponent<FileItem>();

            if (fileItemComponent != null)
            {
                FileInfo fileInfo = await FirestoreManager.Instance.ReadFileInfo(fileId.Trim());
                fileItemComponent.SetFileInfo(fileInfo);
                fileItemInstances[fileId] = newFileItem;
            }
            else
            {
                Debug.LogWarning("FilePrefab does not contain a FileItem component.");
            }
        }

        this.currentFileIds = outputFileIds;
    }
}
