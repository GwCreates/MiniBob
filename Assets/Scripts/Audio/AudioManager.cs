using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource Interact;
    public AudioSource SkipDialogue;
    public AudioSource NewQuest;
    public AudioSource FinishedQuest;

    [Button]
    public void PlayInteractAudio()
    {
        Interact.Play();
    }
    
    [Button]
    public void PlaySkipDialogueAudio()
    {
        SkipDialogue.Play();
    }

    [Button]
    public void PlayNewQuestAudio()
    {
        NewQuest.Play();
    }

    [Button]
    public void PlayFinishedQuestAudio()
    {
        FinishedQuest.Play();
    }
}
