using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectScanInteractable : Interactable
{
    [SerializeField, TextArea] private string description = "Description";
    

    protected override void Interact()
    {
        if (DetectingPlayer && IsInteractable && IsInteractionAllowed && CurrentlyActiveInteractable == this)
        {
            ObjectInfoUI.Instance.UpdateInfo(Title, description);

            base.Interact();
        }
    }
}