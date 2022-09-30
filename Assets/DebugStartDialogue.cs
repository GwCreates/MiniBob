using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugStartDialogue : MonoBehaviour
{
    [Button]
    public void StartDialogue(string conversation)
    {
        DialogueManager.Instance.StartConversation(conversation);
    }
}
