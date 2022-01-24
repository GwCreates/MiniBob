using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Keys : MonoBehaviour
{
    void Start()
    {
        Lua.RegisterFunction("DisableKeys", this, SymbolExtensions.GetMethodInfo(() => DisableObject()));
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("DisableKeys");
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
