using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ProfileText : MonoBehaviour
{
    private TMP_Text _tmpText;

    [SerializeField] private string dialogueVariable;
    [SerializeField] private string titleFalse = "Unknown";
    [FormerlySerializedAs("titletrue")] [SerializeField, TextArea] private string titleTrue;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _tmpText.text = DialogueLua.GetVariable(dialogueVariable).asBool ? titleTrue : titleFalse;
    }
}
