using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void FixedUpdate()
    {
        characterController2D.Move(moveInput * movementSpeed * Time.fixedDeltaTime, false, false);
    }
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();

        controls.Player.Move.started += context => moveInput = context.ReadValue<Vector2>().x;
        controls.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += context => moveInput = context.ReadValue<Vector2>().x;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
