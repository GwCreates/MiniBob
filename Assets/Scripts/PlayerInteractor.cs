using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : Singleton<PlayerInteractor>
{
    [SerializeField] public Interactable currentInteractable = null;
}
