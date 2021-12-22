using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ObjectInfoUI : Singleton<ObjectInfoUI>
{
    [SerializeField] private TMP_Text title = null;
    [SerializeField] private TMP_Text description = null;
    

    [Button]
    public void UpdateInfo(string title, string description)
    {
        this.title.text = title;
        this.description.text = description;
    }
}
