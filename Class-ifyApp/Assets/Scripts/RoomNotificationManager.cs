using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RoomNotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    private float displayDuration = 5f;
    private List<string> activeMessages = new List<string>();
    private readonly object lockObject = new object();

    private void Start()
    {
        if (notificationText == null)
        {
            Debug.LogError("Error: notification text not assigned in editor.");
        }
    }

    // Called when a player joins
    public void ShowPlayerJoined(string playerName)
    {
        DisplayMessage($"{playerName} has joined the room");
    }

    // Called when a player leaves
    public void ShowPlayerLeft(string playerName)
    {
        DisplayMessage($"{playerName} has left the room");
    }

    private void DisplayMessage(string message)
    {
        lock (lockObject)
        {
            activeMessages.Add(message);
            UpdateNotificationText();
        }

        StartCoroutine(RemoveMessageAfterDelay(message, displayDuration));
    }

    private IEnumerator RemoveMessageAfterDelay(string message, float delay)
    {
        yield return new WaitForSeconds(delay);

        lock (lockObject)
        {
            activeMessages.Remove(message);
            UpdateNotificationText();
        }
    }

    private void UpdateNotificationText()
    {
        notificationText.text = string.Join("\n", activeMessages);
    }
}
