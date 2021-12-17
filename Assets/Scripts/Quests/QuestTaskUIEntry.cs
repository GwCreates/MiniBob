using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class QuestTaskUIEntry : MonoBehaviour
{
    [ChildGameObjectsOnly, Required] public TMP_Text QuestTask;
}
