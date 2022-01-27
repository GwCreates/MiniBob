using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuestTab : MonoBehaviour
{
    private Button button;
    public RectTransform dropdownImage;
    public StandardUIQuestTracker QuestTracker;
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();

        controls.Player.CharacterInfoToggle.performed += context => TriggerCharacterInfo();
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent < Button>();
        button.onClick.AddListener(TriggerCharacterInfo);
    }

    void TriggerCharacterInfo()
    {
        if (QuestTracker.isVisible)
            QuestTracker.HideTracker();
        else
            QuestTracker.ShowTracker();

        dropdownImage.localScale = new Vector3(dropdownImage.localScale.x * -1, dropdownImage.localScale.y, dropdownImage.localScale.z);
    }

    private bool previousVisible = true;

    private void LateUpdate()
    {
        previousVisible = QuestTracker.isVisible;
    }

    protected virtual void OnEnable()
    {
        controls.Enable();
        QuestManager.Instance.OnQuestStart.AddListener(EnableQuestTab);
    }

    protected virtual void OnDisable()
    {
        controls.Disable();
    }

    void EnableQuestTab()
    {
        if (!previousVisible)
        {
            TriggerCharacterInfo();
        }
    }
}
