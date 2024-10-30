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
        }
        else
        {
            Debug.LogError("DataHolderMainMenu instance is null.");
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
