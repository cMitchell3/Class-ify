using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using Photon.Voice.Unity;

public class SettingsLogic : MonoBehaviour
{
    public GameObject settingsMenu;
    public PauseLogic pauseLogic;

    // Settings Menu References
    private const string volumePrefKey = "VolumePreference";
    private const string microphoneDevicePrefKey = "MicrophoneDevicePreference";
    public Slider volumeSlider;
    public TMP_Dropdown micDropdown;

    // Mic
    public Recorder microphone;


    void Start()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            int index = micDropdown.value;
            string selectedMic = micDropdown.options[index].text;
            SetMicrophoneInputDevice(selectedMic);
        }

        // Load volume settings
        float savedVolume = PlayerPrefs.GetFloat(volumePrefKey, 1f);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        PopulateMicrophoneDropdown();
        LoadMicrophonePreference();
    }

    void Update()
    {
        
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void PauseToSettingsButton()
    {
        pauseLogic.pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsMenu.SetActive(false);
        pauseLogic.pauseMenu.SetActive(true);
    }

    public void ChangeMicrophoneInputDevice()
    {
        int index = micDropdown.value;
        string selectedMic = micDropdown.options[index].text;

        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            SetMicrophoneInputDevice(selectedMic);
        }
        else
        {
            PlayerPrefs.SetString(microphoneDevicePrefKey, selectedMic); // Save the preference
            PlayerPrefs.Save();
        }
    }

    private void SetMicrophoneInputDevice(string deviceName)
    {
        microphone.MicrophoneDevice = new Photon.Voice.DeviceInfo(deviceName);

        PlayerPrefs.SetString(microphoneDevicePrefKey, deviceName); // Save the preference
        PlayerPrefs.Save();
    }

    void PopulateMicrophoneDropdown()
    {
        micDropdown.ClearOptions();

        string[] microphones = Microphone.devices;

        List<string> options = new List<string>();

        if (microphones.Length > 0)
        {
            options.AddRange(microphones);
        }
        else
        {
            options.Add("No Microphones Found");
        }

        micDropdown.AddOptions(options);

        micDropdown.value = 0;
        micDropdown.RefreshShownValue();
    }

    private void LoadMicrophonePreference()
    {
        string savedMicrophone = PlayerPrefs.GetString(microphoneDevicePrefKey, string.Empty);

        if (!string.IsNullOrEmpty(savedMicrophone))
        {
            int index = micDropdown.options.FindIndex(option => option.text == savedMicrophone);
            if (index != -1)
            {
                micDropdown.value = index;
            }
        }
    }

    public void OpenAudioOutputSettings()
    {
        Process.Start("ms-settings:sound");
    }

    public void UpdateVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(volumePrefKey, volume);
        PlayerPrefs.Save();
    }
}
