using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text InteractionText;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Interactable.CurrentlyActiveInteractable != null && Interactable.CurrentlyActiveInteractable.IsInteractable && Interactable.IsInteractionAllowed)
        {
            
            InteractionText.text = Interactable.CurrentlyActiveInteractable.Title;
            if (string.IsNullOrEmpty(InteractionText.text))
                InteractionText.text = Interactable.CurrentlyActiveInteractable.name;
                
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Reset()
    {
        InteractionText = GetComponentInChildren<TMP_Text>();
    }
}
