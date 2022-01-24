using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButtonShortcut : MonoBehaviour
{
    private Button button;
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();

        controls.UI.ContinueDialogue.performed += context => button.OnSubmit(null);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
