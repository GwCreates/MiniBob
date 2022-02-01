using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;
    void Start()
    {

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    [SerializeField] private LayerMask layerMask = 128;

    [SerializeField] private List<GameObject> entries;
    
    [Button]
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OPen??? + " + !(DialogueManager.IsConversationActive && LayerMask.LayerToName(other.gameObject.layer) == "Player"));
        Debug.Log("conversation active??? + " + (DialogueManager.IsConversationActive));
        Debug.Log("is player??? + " + (LayerMask.LayerToName(other.gameObject.layer) == "Player"));
        Debug.Log("Collide! + " + LayerMask.LayerToName(other.gameObject.layer), other);
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("Open door");
            if (!entries.Contains(other.gameObject))
            {
                entries.Add(other.gameObject);
            }

            if (!(DialogueManager.IsConversationActive && LayerMask.LayerToName(other.gameObject.layer) == "Player"))
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        if (!_animator.GetBool("IsOpen"))
        {
            _animator.SetBool("IsOpen", true);
            _audioSource.Play();
        }
    }

    private void CloseDoor()
    {
        if (_animator.GetBool("IsOpen"))
        {
            _animator.SetBool("IsOpen", false);
            _audioSource.PlayDelayed(0.3f);
        }
    }

    private void Update()
    {
        if (!DialogueManager.IsConversationActive && entries.Count > 0)
        {
            OpenDoor();
        }
    }

    [Button]
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Collide! + " + LayerMask.LayerToName(other.gameObject.layer), other);
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("Open door");
            
            if (entries.Contains(other.gameObject))
            {
                entries.Remove(other.gameObject);
            }

            if (entries.Count == 0)
            {
                CloseDoor();
            }
        }
    }
}
