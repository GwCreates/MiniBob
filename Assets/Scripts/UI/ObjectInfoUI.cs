using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ObjectInfoUI : Singleton<ObjectInfoUI>
{
    [SerializeField, FoldoutGroup("References")] private GameObject infoParent = null;
    [SerializeField, FoldoutGroup("References")] private TMP_Text title = null;
    [SerializeField, FoldoutGroup("References")] private TMP_Text description = null;

    private void Awake()
    {
        infoParent.SetActive(false);
    }

    [Button, DisableInEditorMode]
    public void UpdateInfo(string title, string description)
    {
        infoParent.SetActive(true);
        this.title.text = title;
        this.description.text = description;
    }

    [Button, DisableInEditorMode]
    public void CloseInfo()
    {
        infoParent.SetActive(false);
    }
}
