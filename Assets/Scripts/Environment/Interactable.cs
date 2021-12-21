using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] public string Title = "";
    [SerializeField] public bool IsInteractable = true;
    public static bool IsInteractionAllowed => !DialogueManager.IsConversationActive;
    [SerializeField] private LayerMask layerMask = 128;
    public static Interactable CurrentlyActiveInteractable;
    public static List<Interactable> InteractableQueue = new List<Interactable>();
    
    private PlayerInput controls;

    [SerializeField, Range(0, 100)] public int priority = 100;

    [SerializeField] private UnityEvent OnPlayerEnter = new UnityEvent();
    [SerializeField] private UnityEvent OnInteract = new UnityEvent();
    [SerializeField] private UnityEvent OnPlayerExit = new UnityEvent();
    
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
        if (DetectingPlayer && IsInteractable && IsInteractionAllowed && CurrentlyActiveInteractable == this)
            OnInteract.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
        PickNewInteractable();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInteractable && !DetectingPlayer && layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            DetectingPlayer = true;
            if (priority == 100 || CurrentlyActiveInteractable == null || CurrentlyActiveInteractable.priority <= priority)
            {
                if (CurrentlyActiveInteractable != null)
                {
                    CurrentlyActiveInteractable.OnPlayerLeft(other);
                    InteractableQueue.Add(CurrentlyActiveInteractable);
                }
                if (!IsInteractable)
                    return;
        
                OnPlayerEnter?.Invoke();

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
        
        if (ControlPrompt != null)
            ControlPrompt?.SetActive(true);
        Player = other.transform.parent;
        DetectingPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            // Remove oneself from queue
            if (InteractableQueue.Contains(this))
            {
                InteractableQueue.Remove(this);
            }
            
            // Select new active interactable
            if (CurrentlyActiveInteractable == this)
            {
                CurrentlyActiveInteractable = null;
                PickNewInteractable(other);
            }
            
            OnPlayerExit?.Invoke();
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
        
        if (ControlPrompt != null)
            ControlPrompt.SetActive(false);
        Player = null;
        DetectingPlayer = false;
    }

    private static void PickNewInteractable(Collider2D other)
    {
        if (InteractableQueue.Count <= 0)
            return;

        PickNewInteractable();
        if (CurrentlyActiveInteractable != null)
            CurrentlyActiveInteractable.OnPlayerEntered(other);
    }

    public static void PickNewInteractable()
    {
        if (InteractableQueue.Count <= 0)
            return;
        
        InteractableQueue.Sort((a,b) => a.priority.CompareTo(b.priority));
        CurrentlyActiveInteractable = InteractableQueue.Find((x) => x.IsInteractable); // InteractableQueue[0];
    }

    public void SetIsInteractable(bool isInteractable)
    {
        this.IsInteractable = isInteractable;
        if (isInteractable == false)
        {
            if (ControlPrompt != null)
                ControlPrompt.SetActive(false);
            
            if (InteractableQueue.Contains(this))
                InteractableQueue.Remove(this);
            if (CurrentlyActiveInteractable == this)
                CurrentlyActiveInteractable = null;
        }

        PickNewInteractable();
    }


    protected virtual void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
