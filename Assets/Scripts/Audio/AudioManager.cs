using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource Interact;

    [Button]
    public void PlayInteractAudio()
    {
        Interact.Play();
    }
}
