using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField, ChildGameObjectsOnly] private TMP_Text QuestTitle;
    [SerializeField, ChildGameObjectsOnly] private List<QuestTaskUIEntry> questTaskUIEntries = new List<QuestTaskUIEntry>();

    [SerializeField] private Item currentQuest = null;

    [Button]
    public void SetActiveQuest(string questName)
    {
        currentQuest = DialogueManager.Instance.initialDatabase.GetItem(questName);
    }

    [Button(25), DisableInEditorMode]
    public void UpdateQuestUI()
    {
        if (currentQuest == null || string.IsNullOrEmpty(currentQuest.Name))
            return;

        QuestTitle.text = currentQuest.Name;

        int entryCount = int.Parse(currentQuest.AssignedField("Entry Count").value);
        Debug.Log("Entry Count: " + entryCount);

        for (int i = 0; i < questTaskUIEntries.Count; i++)
        {
            if (i < entryCount)
            {
                // Show Quest Entry
                questTaskUIEntries[i].gameObject.SetActive(true);
                string entryName = "Entry " + (i + 1);
                questTaskUIEntries[i].QuestTask.text = currentQuest.AssignedField(entryName).value;
                QuestState questState = QuestLog.StringToState(currentQuest.AssignedField(entryName + " State").value);
                
                if (questState == QuestState.Success)
                {
                    questTaskUIEntries[i].QuestTask.fontStyle |= FontStyles.Strikethrough;
                }
                else
                {
                    questTaskUIEntries[i].QuestTask.fontStyle &= ~FontStyles.Strikethrough;
                }
            }
            else
            {
                // Hide UI Element
                questTaskUIEntries[i].gameObject.SetActive(false);
            }
        }
    }
    
    
    private void Reset()
    {
        QuestTitle = GetComponentInChildren<TMP_Text>();
        questTaskUIEntries = GetComponentsInChildren<QuestTaskUIEntry>().ToList();
    }
}
