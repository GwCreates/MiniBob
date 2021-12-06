using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] private bool DetectingPlayer = false;
    [SerializeField] private Transform TargetPosition = null;
    [SerializeField] private Transform Player = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("yerd");
        if (other.CompareTag("Player"))
        {
            Player = other.transform;
            DetectingPlayer = true;
            Debug.Log("Player Entered");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = null;
            DetectingPlayer = false;
            Debug.Log("Player Left");
        }
    }

    private void Update()
    {
        if (DetectingPlayer)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Player.position = TargetPosition.position;
            }
        }
    }
}
