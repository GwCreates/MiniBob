using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource Interact;
    public AudioSource SkipDialogue;
    public AudioSource NewQuest;
    public AudioSource FinishedQuest;
    public AudioSource Music;
    void Start()
    {
        Lua.RegisterFunction("PlayMusic", this, SymbolExtensions.GetMethodInfo(() => PlayMusicAudio()));
        Lua.RegisterFunction("StopMusic", this, SymbolExtensions.GetMethodInfo(() => StopMusicAudio()));
    }

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

    [Button]
    public void PlayMusicAudio()
    {
        if (!Music.isPlaying)
            Music.Play();
    }
    [Button]
    public void StopMusicAudio()
    {
        Music.Stop();
    }
}
