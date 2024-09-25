using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginInputManager : MonoBehaviour
{
    // Input Fields
    public TMP_InputField emailUsernameInputField;
    public TMP_InputField passwordInputField;


    void Start()
    {
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
