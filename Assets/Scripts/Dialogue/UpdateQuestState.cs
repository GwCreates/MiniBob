using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;

public class UpdateQuestState : MonoBehaviour
{
    [SerializeField, TextArea] private string questName = "";
    [SerializeField] private int entry = 1;

    [Button, DisableInEditorMode]
    public void UpdateQuest(string questName, QuestState questState)
    {
        QuestLog.SetQuestState(questName, questState);
    }
    
    public void SetQuestActive()
    {
        QuestLog.SetQuestState(questName, QuestState.Active);
    }

    public void SetQuestSuccess()
    {
        QuestLog.SetQuestState(questName, QuestState.Success);
    }
    
    [Button, DisableInEditorMode]
    public void UpdateQuestEntry(string questName, int entry, QuestState questState)
    {
        QuestLog.SetQuestEntryState(questName, entry, questState);
    }

    public void SetQuestEntryActive()
    {
        QuestLog.SetQuestEntryState(questName, entry, QuestState.Active);
    }

    public void SetQuestEntrySuccess()
    {
        QuestLog.SetQuestEntryState(questName, entry, QuestState.Success);
    }
}
