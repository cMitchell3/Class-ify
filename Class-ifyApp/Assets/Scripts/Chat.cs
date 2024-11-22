using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    public TMP_InputField chatInput;
    public GameObject message;
    public GameObject content;

    void Update()
    {
        if (chatInput.isFocused)
        {
            Debug.Log("The Input Field Is Focused");
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SendMessage();
            }
        }
    }

    // Called when a player joins
    public void ShowPlayerJoined(string playerName)
    {
        SendMessage($"{playerName} has joined the room");
    }

    // Called when a player leaves
    public void ShowPlayerLeft(string playerName)
    {
        SendMessage($"{playerName} has left the room");
    }

    public void SendMessage()
    {
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, chatInput.text);
        chatInput.text = "";
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SendMessage(string message)
    {
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, message);
        chatInput.text = "";
        EventSystem.current.SetSelectedGameObject(null);
    }

    [PunRPC]
    public void GetMessage(string receiveMessage)
    {
        GameObject m = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        m.GetComponent<Message>().myMessage.text = receiveMessage;
    }
}
