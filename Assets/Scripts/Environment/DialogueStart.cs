using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[RequireComponent(typeof(DialogueSystemTrigger))]
public class DialogueStart : Interactable
{
   
    [SerializeField] DialogueSystemTrigger dialogueStart;

    protected override void Interact()
    {
        if (DetectingPlayer && IsInteractable && IsInteractionAllowed && CurrentlyActiveInteractable == this)
        {
          
            dialogueStart.OnUse();

            base.Interact();
        }
    }

    protected override void Reset()
    {
        base.Reset();
        dialogueStart = GetComponent<DialogueSystemTrigger>();
    }
}