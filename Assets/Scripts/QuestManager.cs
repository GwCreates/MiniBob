using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private List<string> activeQuests = new List<string>();

    // Update is called once per frame
    void LateUpdate()
    {
        CheckNewActiveQuests();
        
        CheckActiveQuestsCompletion();
    }

    private void CheckNewActiveQuests()
    {
        foreach (var quest in QuestLog.GetAllQuests())
        {
            if (QuestLog.IsQuestActive(quest) && !activeQuests.Contains(quest))
            {
                activeQuests.Add(quest);
            }
        }
    }

    private void CheckActiveQuestsCompletion()
    {
        // Find all completed quests that haven't been marked completed yet
        List<string> completedQuests = new List<string>();
        foreach (var activeQuest in activeQuests)
        {
            int entryCount = QuestLog.GetQuestEntryCount(activeQuest);
            if (entryCount == 0)
                continue;
            bool questFinished = true;
            for (int i = 1; i < entryCount+1; i++)
            {
                if (QuestLog.GetQuestEntryState(activeQuest, i) != QuestState.Success)
                {
                    questFinished = false;
                    break;
                }
            }
            
            if (questFinished)
                completedQuests.Add(activeQuest);
        }

        // Actually complete all completed quests
        foreach (var completedQuest in completedQuests)
        {
            CompleteQuest(completedQuest);
        }
    }

    public void StartQuest(string questName)
    {
        QuestLog.StartQuest(questName);
        activeQuests.Add(questName);
    }

    public void CompleteQuest(string questName)
    {
        QuestLog.CompleteQuest(questName);
        activeQuests.Remove(questName);
    }

    public void CompleteQuestEntry(string questName, int entry)
    {
        QuestLog.SetQuestEntryState(questName, entry, QuestState.Success);
        CheckActiveQuestsCompletion();
    }
}
