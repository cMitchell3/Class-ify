using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomCodeDisplayController : MonoBehaviour
{
    public TextMeshProUGUI roomCodeDisplay;

    private string joinCode = "";

    // On startup get room code and initialize text
    void Awake() {
        if (DataHolderMainMenu.Instance != null)
        {
            joinCode = DataHolderMainMenu.Instance.savedCode;
        }
        else
        {
            Debug.LogError("DataHolderMainMenu instance is null.");
        }

        if (roomCodeDisplay == null)
        {
            roomCodeDisplay = GetComponent<TextMeshProUGUI>();
        }

        UpdateText();
    }

    // Updates the currency UI number
    private void UpdateText()
    {
        roomCodeDisplay.text = "Join Code: " + joinCode;
    }
}
