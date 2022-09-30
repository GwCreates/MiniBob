using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Singleton<PlayerMovement>
{
    private CharacterController2D characterController2D;
    private CharacterController characterController;

    [SerializeField] private Vector2 movementSpeed = new Vector2(25f, 1f);
    Vector2 moveInput = Vector2.zero;

    public CameraTrigger currentRoom;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController2D = GetComponent<CharacterController2D>();
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (DialogueManager.Instance.IsConversationActive && DialogueManager.Instance.ConversationController.currentState.pcResponses.Length > 1)
        {
            characterController2D.Move(0f, 0f, false);
        }
        else
        {
            characterController2D.Move(moveInput.x * movementSpeed.x * Time.fixedDeltaTime, moveInput.y * movementSpeed.y * Time.fixedDeltaTime, false);
        }
    }
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();

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
