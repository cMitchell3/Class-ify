using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateRoomMenuLogic : MonoBehaviour
{
    // Inputs and Fields
    public TMP_InputField userCountInput;
    public Button plusButton;
    public Button minusButton;

    // Values
    private int currentUsers = 1;
    private const int minUsers = 1;
    private const int maxUsers = 16;
    

    void Start()
    {
        userCountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
        userCountInput.text = currentUsers.ToString();

        plusButton.onClick.AddListener(IncrementUsers);
        minusButton.onClick.AddListener(DecrementUsers);

        userCountInput.onEndEdit.AddListener(OnInputValueChanged);
    }

    void IncrementUsers()
    {
        if (currentUsers < maxUsers)
        {
            currentUsers++;
            UpdateInputField();
        }
    }

    void DecrementUsers()
    {
        if (currentUsers > minUsers)
        {
            currentUsers--;
            UpdateInputField();
        }
    }

    void UpdateInputField()
    {
        userCountInput.text = currentUsers.ToString();
    }

    void OnInputValueChanged(string value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            currentUsers = Mathf.Clamp(parsedValue, minUsers, maxUsers);
        }
        else
        {
            userCountInput.text = currentUsers.ToString();
        }

        UpdateInputField();
    }
}
