using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueStart : Interactable
{
    [SerializeField] private Transform TargetPosition = null;
    [SerializeField] DialogueSystemTrigger dialogueStart;

    protected override void Interact()
    {
        if (DetectingPlayer)
        {
            Player.position = TargetPosition.position;
            dialogueStart.OnUse();

            base.Interact();
        }
    }
}