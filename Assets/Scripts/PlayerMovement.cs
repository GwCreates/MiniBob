using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D characterController2D;
    private CharacterController characterController;

    [SerializeField] private Vector2 movementSpeed = new Vector2(25f, 1f);
    Vector2 moveInput = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController2D = GetComponent<CharacterController2D>();
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        characterController2D.Move(moveInput.x * movementSpeed.x * Time.fixedDeltaTime, moveInput.y * movementSpeed.y * Time.fixedDeltaTime, false);
    }
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();
        Debug.Log("Awake");

        controls.Player.Move.started += context => moveInput = context.ReadValue<Vector2>();
        controls.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => moveInput = context.ReadValue<Vector2>();
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
