using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFB;
using System.IO;

public class ExportFile : MonoBehaviour
{
    private static int exportCoins = 20;

    public static void SaveFile(string fileName, string extension, byte[] content, string uploadUser)
    {
        Debug.Log("Opening save file dialog");
        string savePath = SaveFileDialog(fileName, extension);

        if (!savePath.Equals(""))
        {
            File.WriteAllBytes(savePath, content);
            Debug.Log("File saved at: " + savePath);

            if (!FirebaseAuthManager.Instance.GetUserEmail().Equals(uploadUser))
            {
                FirestoreManager.Instance.UpdateUserCurrency(uploadUser, exportCoins);
            }
        }
    }

    private static string SaveFileDialog(string fileName, string extension)
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extension);

        return !string.IsNullOrEmpty(path) ? path : string.Empty;
    }
}
