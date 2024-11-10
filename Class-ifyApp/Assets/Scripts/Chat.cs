using TMPro;
using UnityEngine;
using Photon.Pun;

public class Chat : MonoBehaviour
{
    public TMP_InputField chatInput;
    public GameObject message;
    public GameObject content;
    

    public void SendMessage()
    {
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, chatInput.text);
    }

    [PunRPC]
    public void GetMessage(string receiveMessage)
    {
        GameObject m = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        m.GetComponent<Message>().myMessage.text = receiveMessage;
    }
}
