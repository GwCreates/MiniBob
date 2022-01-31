using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {

        _animator = GetComponent<Animator>();
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
                _animator.SetBool("IsOpen", true);
        }
    }

    private void Update()
    {
        if (!DialogueManager.IsConversationActive && entries.Count > 0)
        {
            _animator.SetBool("IsOpen", true);
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
                _animator.SetBool("IsOpen", false);
            }
        }
    }
}
