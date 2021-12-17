using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuestTrackerToggle : MonoBehaviour
{
    [SerializeField] private StandardUIQuestTracker questTracker = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    void UpdateQuestTracker()
    {
        questTracker.UpdateTracker();
    }

    private void Reset()
    {
        questTracker = GetComponent<StandardUIQuestTracker>();
    }
}
