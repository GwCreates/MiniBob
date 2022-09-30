using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ForceSelect : MonoBehaviour
{
    private Selectable Selectable
    {
        get
        {
            if (_selectable == null)
                _selectable = GetComponent<Selectable>();
            return _selectable;
        }
    }
    private Selectable _selectable;

    private void OnEnable()
    {
        Selectable.Select();
    }
}
