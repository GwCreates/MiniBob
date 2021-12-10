using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueStart : Interactable
{
   
    [SerializeField] DialogueSystemTrigger dialogueStart;

    protected override void Interact()
    {
        if (DetectingPlayer)
        {
          
            dialogueStart.OnUse();

            base.Interact();
        }
    }
}