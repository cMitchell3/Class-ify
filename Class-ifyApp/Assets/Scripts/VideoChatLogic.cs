using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoChatLogic : MonoBehaviour
{
    public GameObject videoChatPanel;

    public void ActivateVideoChatPanel()
    {
        videoChatPanel.SetActive(!videoChatPanel.activeSelf);
    }

    public void LeaveVoiceChat()
    {
        videoChatPanel.SetActive(false);
    }
}
