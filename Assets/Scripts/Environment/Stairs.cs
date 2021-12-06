using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stairs : Interactable
{
    [SerializeField] private Transform TargetPosition = null;
    

    protected override void Interact()
    {
        if (DetectingPlayer)
        {
            Player.position = TargetPosition.position;
            
            base.Interact();
        }
    }
}
