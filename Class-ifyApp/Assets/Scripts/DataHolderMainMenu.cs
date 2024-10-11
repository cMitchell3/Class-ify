using TMPro;
using UnityEngine;

public class DataHolderMainMenu : MonoBehaviour
{
    public static DataHolderMainMenu Instance;
    public string savedCode;

    // Ensuring only one instance of the singleton exists and is not destroyed when switching scenes
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update the savedText when code updates
    public void UpdateSavedCode(string newCode)
    {
        Debug.Log("Saved code: " + newCode);
        savedCode = newCode;
    }
}