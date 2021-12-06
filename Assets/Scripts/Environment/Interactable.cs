using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public static Interactable CurrentlyActiveInteractable;
    public static List<Interactable> InteractableQueue = new List<Interactable>();
    
    private PlayerInput controls;

    [SerializeField, Range(0, 100)] public int priority = 100;

    [SerializeField] private UnityEvent OnInteract = new UnityEvent();
    
    [SerializeField] private GameObject ControlPrompt = null;
    [SerializeField] protected bool DetectingPlayer = false;
    [SerializeField] protected Transform Player = null;
    
    void Awake()
    {
        controls = new PlayerInput();

        controls.Player.Interact.performed += context => Interact();
    }

    protected virtual void Interact()
    {
        OnInteract.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!DetectingPlayer && other.CompareTag("Player"))
        {
            if (priority == 100 || CurrentlyActiveInteractable == null || CurrentlyActiveInteractable.priority <= priority)
            {
                if (CurrentlyActiveInteractable != null)
                {
                    CurrentlyActiveInteractable.OnPlayerLeft(other);
                    InteractableQueue.Add(CurrentlyActiveInteractable);
                }

                CurrentlyActiveInteractable = this;
                OnPlayerEntered(other);
            }
            else
            {
                // Add oneself to queue
                if (!InteractableQueue.Contains(this))
                {
                    InteractableQueue.Add(this);
                }
            }
        }
    }

    public virtual void OnPlayerEntered(Collider2D other)
    {
        // Remove oneself from queue
        if (InteractableQueue.Contains(this))
        {
            InteractableQueue.Remove(this);
        }
        
        ControlPrompt?.SetActive(true);
        Player = other.transform;
        DetectingPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Remove oneself from queue
            if (InteractableQueue.Contains(this))
            {
                InteractableQueue.Remove(this);
            }
            
            // Select new active interable
            if (CurrentlyActiveInteractable == this)
            {
                CurrentlyActiveInteractable = null;
                PickNewInteractable(other);
            }
            
            OnPlayerLeft(other);
        }
    }

    public virtual void OnPlayerLeft(Collider2D other)
    {
        // Remove oneself from queue
        if (InteractableQueue.Contains(this))
        {
            InteractableQueue.Remove(this);
        }
        
        ControlPrompt?.SetActive(false);
        Player = null;
        DetectingPlayer = false;
    }

    private void PickNewInteractable(Collider2D other)
    {
        if (InteractableQueue.Count <= 0)
            return;
        
        InteractableQueue.Sort((a,b) => a.priority.CompareTo(b.priority));
        CurrentlyActiveInteractable = InteractableQueue[0];
        CurrentlyActiveInteractable.OnPlayerEntered(other);
    }
}
