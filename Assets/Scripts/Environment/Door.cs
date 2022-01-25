using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int standardLayer = 10;
    [SerializeField] private int conversationLayer = 9;
    

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("conversation active: " + DialogueManager.Instance.IsConversationActive);
        gameObject.layer = DialogueManager.Instance.IsConversationActive ? conversationLayer : standardLayer;
    }
}
