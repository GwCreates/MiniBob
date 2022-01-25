using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

[RequireComponent(typeof(DialogueStart))]
public class DialogueStartTitleChange : MonoBehaviour
{

    private DialogueStart _dialogueStart;

    [SerializeField] private string dialogueVariable;
    [SerializeField] private string titleFalse;
    [SerializeField] private string titletrue;
    
    // Start is called before the first frame update
    void Start()
    {
        _dialogueStart = GetComponent<DialogueStart>();
    }

    // Update is called once per frame
    void Update()
    {
        _dialogueStart.Title = DialogueLua.GetVariable(dialogueVariable).asBool ? titletrue : titleFalse;
    }
}
