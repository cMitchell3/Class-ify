using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomCodeDisplayController : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    private string joinCode = "";

    // On startup get room code and initialize text
    public void Start() {
        if (DataHolderMainMenu.Instance != null)
        {
            joinCode = DataHolderMainMenu.Instance.savedCode;

            Debug.Log("The saved room code is: " + joinCode);
        }
        else
        {
            Debug.LogError("DataHolderMainMenu instance is null! Make sure it's present in the main menu scene.");
        }

        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        UpdateText();
    }

    // Updates the currency UI number
    private void UpdateText()
    {
        tmpText.text = "Join Code: " + joinCode;
    }
}
