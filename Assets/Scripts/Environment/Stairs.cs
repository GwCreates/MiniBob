using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stairs : Interactable
{
    [SerializeField] public Transform TargetPosition = null;
    

    protected override void Interact()
    {
        if (DetectingPlayer && IsInteractable && IsInteractionAllowed && CurrentlyActiveInteractable == this)
        {
            CapsuleCollider2D capsuleCollider2D = null;
            if (Player.TryGetComponent(out capsuleCollider2D))
            {
                Player.position = TargetPosition.position + Vector3.up * (capsuleCollider2D.size.y - capsuleCollider2D.size.x / 2);
                Debug.Log("Offset: " +  (capsuleCollider2D.size.y - capsuleCollider2D.size.x / 2));
            }
            else
            {
                Player.position = TargetPosition.position + Vector3.up * 1.5f;
            }
            
            base.Interact();
        }
    }
}
