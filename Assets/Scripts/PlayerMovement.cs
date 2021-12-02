using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D characterController2D;
    private CharacterController characterController;

    [SerializeField] private float movementSpeed = 5f;
    float moveInput = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController2D = GetComponent<CharacterController2D>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput--;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput++;
        }
        //characterController.Move(Vector3.right * moveInput * movementSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        characterController2D.Move(moveInput * movementSpeed * Time.fixedDeltaTime, false, false);
    }
}
