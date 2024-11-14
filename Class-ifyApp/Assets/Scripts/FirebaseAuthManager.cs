using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager Instance { get; private set; }
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeFirebaseAuth();
    }

    void InitializeFirebaseAuth()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        if (auth == null)
        {
            Debug.LogError("Error: Failed to connected to Firebase Auth");
        }

        if (user == null)
        {
            Debug.LogError("Error: User not logged in.");
        }
    }

    public string GetUserEmail()
    {
        return user.Email;
    }
}