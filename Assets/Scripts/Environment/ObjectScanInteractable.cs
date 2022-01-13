using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectScanInteractable : Interactable
{
    [SerializeField, TextArea] private string description = "Description";
    

    protected override void OnEnable()
    {
        base.OnEnable();
        
        OnPlayerExit.AddListener(CloseInfo);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnPlayerExit.RemoveListener(CloseInfo);
    }

    protected override void Interact()
    {
        if (DetectingPlayer && IsInteractable && IsInteractionAllowed && CurrentlyActiveInteractable == this)
        {
            ObjectInfoUI.Instance.UpdateInfo(Title, description);

            base.Interact();
        }
    }

    void CloseInfo()
    {
        ObjectInfoUI.Instance.CloseInfo();
    }
}