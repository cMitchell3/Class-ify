using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFB;
using System.IO;

public class ExportFile : MonoBehaviour
{
    public static void SaveFile(string fileName, string extension, byte[] content)
    {
        Debug.Log("Opening save file dialog");
        string savePath = SaveFileDialog(fileName, extension);

        if (!savePath.Equals(""))
        {
            File.WriteAllBytes(savePath, content);
            Debug.Log("File saved at: " + savePath);
        }
    }

    private static string SaveFileDialog(string fileName, string extension)
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extension);

        return !string.IsNullOrEmpty(path) ? path : string.Empty;
    }
}
