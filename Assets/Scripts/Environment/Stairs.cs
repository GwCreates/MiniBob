using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] private List<KeyCode> Keys = new List<KeyCode>(2) { KeyCode.W, KeyCode.UpArrow };

    [SerializeField] private bool DetectingPlayer = false;
    [SerializeField] private Transform TargetPosition = null;
    [SerializeField] private Transform Player = null;
    
    [SerializeField] private GameObject ControlPrompt = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("yerd");
        if (other.CompareTag("Player"))
        {
            ControlPrompt.SetActive(true);
            Player = other.transform;
            DetectingPlayer = true;
            Debug.Log("Player Entered");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ControlPrompt.SetActive(false);
            Player = null;
            DetectingPlayer = false;
            Debug.Log("Player Left");
        }
    }

    private void Update()
    {
        if (DetectingPlayer)
        {
            foreach (var key in Keys)
            {
                if (Input.GetKeyUp(key))
                {
                    Player.position = TargetPosition.position;
                }
            }
        }
    }
}
