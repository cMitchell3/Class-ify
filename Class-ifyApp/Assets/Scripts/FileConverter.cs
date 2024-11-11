using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileConverter : MonoBehaviour
{
    public static string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);
        
        return base64String;
    }

    public static void ConvertBase64ToFile(string base64String, string outputFilePath)
    {
        byte[] fileBytes = Convert.FromBase64String(base64String);
        File.WriteAllBytes(outputFilePath, fileBytes);
        
        UnityEngine.Debug.Log("File saved to: " + outputFilePath);
    }
}
