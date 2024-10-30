using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MuteMicrophone : MonoBehaviour
{
    // Image variables
    public Sprite micUnmuted;
    public Sprite micMuted;
    public Image micImage;

    // Microphone
    public Recorder recorder;
    public TMP_Text errorMessage;
    private GameObject noMicText;
    

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            noMicText = GameObject.FindGameObjectWithTag("NoMicText");
            errorMessage = noMicText.GetComponent<TMP_Text>();
            Debug.Log("We are good!");
        }
    }

    void Update()
    {
        CheckMicrophone();
    }

    public void MuteMicrophoneButton()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            micImage.sprite = micImage.sprite == micUnmuted ? micMuted : micUnmuted;
        }
    }

    public void CheckMicrophone()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            if (errorMessage == null)
            {
                noMicText = GameObject.FindGameObjectWithTag("NoMicText");
                errorMessage = noMicText.GetComponent<TMP_Text>();
                Debug.LogWarning("Error message Text UI is not assigned.");
                return;
            }
            if (Microphone.devices.Length == 0)
            {
                errorMessage.text = "Error - no microphone connected!";
                errorMessage.color = Color.red;
                errorMessage.gameObject.SetActive(true);
            }
            else
            {
                errorMessage.text = "";
                errorMessage.gameObject.SetActive(false);
            }
        }
    }
}
