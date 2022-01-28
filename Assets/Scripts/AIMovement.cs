using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public DialogueStart DialogueStart;
    public string name = "Female";

    public bool female = true;
    public Transform CurrentTarget;
    public int currentFloor = 0;

    private bool shouldWalkAfterConversation = false;
    private string nextConversation = "";

    private Animator animator;

    private bool targetingPlayer = true;
    [ShowInInspector] private CameraTrigger targetRoom = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
        characterController2D = GetComponent<CharacterController2D>();
        animator = GetComponentInChildren<Animator>();
        Lua.RegisterFunction("Move" + name + "ToPlayer", this, SymbolExtensions.GetMethodInfo(() => WalkToPlayer(string.Empty)));
        Lua.RegisterFunction("Move" + name + "ToRoom", this, SymbolExtensions.GetMethodInfo(() => MoveToRoom(string.Empty)));
        Lua.RegisterFunction("Move" + name + "ToRoomDialogue", this, SymbolExtensions.GetMethodInfo(() => MoveToRoom(string.Empty, string.Empty)));
        Lua.RegisterFunction("Set" + name + "AnimationBool", this, SymbolExtensions.GetMethodInfo(() => SetAnimationBool(string.Empty, false)));
    }

    public string SpeakerName = "Female";
    
    void Update()
    {
        if (shouldWalkAfterConversation && !DialogueManager.IsConversationActive)
        {
            WalkToPlayer(nextConversation);
        }


        if (DialogueManager.IsConversationActive)
        {
            Debug.Log(DialogueManager.currentConversationState.subtitle.speakerInfo.Name);
            // Debug.Log(
            //     "Active Actor: " + DialogueManager.currentConversationState.subtitle.speakerInfo.transform + " Parent: " +
            //     DialogueManager.currentConversationState.subtitle.speakerInfo.transform.parent.name,
            //     DialogueManager.currentConversationState.subtitle.speakerInfo.transform);
            if (DialogueManager.currentConversationState.subtitle.speakerInfo.Name == SpeakerName)
            {
                SetAnimationBool("IsTalking", true);
            }
            else
            {
                SetAnimationBool("IsTalking", false);
            }
        }
        else
        {
            SetAnimationBool("IsTalking", false);
        }
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("Move" + name + "ToPlayer");
    }

    private CharacterController2D characterController2D;

    [SerializeField] private Vector2 movementSpeed = new Vector2(25f, 1f);

    void MoveToRoom(string room)
    {
        MoveToRoom(room, "");
    }

    private string targetedRoom = "";
    
    [Button]
    void MoveToRoom(string room, string conversation)
    {
        targetingPlayer = false;
        if (!string.IsNullOrEmpty(room))
        {
            targetedRoom = room;
            CameraTrigger selectedRoom = null;
            CameraTrigger[] rooms = FindObjectsOfType<CameraTrigger>();
            foreach (var roomTrigger in rooms)
            {
                if (roomTrigger.name.Contains(room))
                {
                    selectedRoom = roomTrigger;
                    break;
                }
            }


            if (selectedRoom != null)
            {
                targetRoom = selectedRoom;
                
                if (selectedRoom.floor != currentFloor)
                {
                    if (currentFloor == 0)
                    {
                        CurrentTarget = AIManager.Instance.stairsFloor0.transform;
                    }
                    else
                    {
                        CurrentTarget = AIManager.Instance.stairsFloor1.transform;
                    }
                }
                else
                {
                    CurrentTarget = female ? selectedRoom.targetPositionFemale : selectedRoom.targetPositionMale;
                }

                StartCoroutine(WalkToPlayerCoroutine(conversation));
            }
            else
            {
                Debug.LogError("Room was not found: " + room, this);
            }
        }
    }

    [Button]
    void WalkToPlayer(string conversation)
    {
        targetingPlayer = true;
        DialogueStart.enabled = false;
        Debug.LogWarning("Walk to Player + " + conversation, this);
        if (DialogueManager.IsConversationActive && !string.IsNullOrEmpty(conversation))
        {
            shouldWalkAfterConversation = true;
            nextConversation = conversation;
            
        }
        else
        {
            shouldWalkAfterConversation = false;
            FindTarget(string.IsNullOrEmpty(conversation));

            StartCoroutine(WalkToPlayerCoroutine(conversation));
        }
    }

    private void FindTarget(bool roomOnly = false)
    {
        if (PlayerMovement.Instance.currentRoom.floor != currentFloor)
        {
            if (currentFloor == 0)
            {
                targetRoom = AIManager.Instance.stairsFloor0Room;
                CurrentTarget = AIManager.Instance.stairsFloor0.transform;
            }
            else
            {
                targetRoom = AIManager.Instance.stairsFloor1Room;
                CurrentTarget = AIManager.Instance.stairsFloor1.transform;
            }
        }
        else
        {
            CurrentTarget = female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;
            if (targetingPlayer && !roomOnly)
            {
                if (Vector2.Distance(transform.position, new Vector2(CurrentTarget.position.x, transform.position.y)) <
                    Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)))
                {
                    CurrentTarget = PlayerMovement.Instance.transform;// female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;

                    targetRoom = PlayerMovement.Instance.currentRoom;
                }
            }
            else
            {
                CurrentTarget = female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;
                targetRoom = PlayerMovement.Instance.currentRoom;
            }
            // CurrentTarget = PlayerMovement.Instance.transform;// female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;
            // CurrentTarget = female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;
        }
    }

    private IEnumerator WalkToPlayerCoroutine(string conversation)
    {
        Stairs stair;
        bool targetIsStair = CurrentTarget.TryGetComponent(out stair);
        bool roomOnly = string.IsNullOrEmpty(conversation);
        
        // Debug.Log("targetIsStair? " + targetIsStair);
        // Debug.Log("Close To Player? " + !(!targetIsStair && Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)) < 1f));
        // Debug.Log("Close To target? " + (Vector2.Distance(transform.position, new Vector2(CurrentTarget.position.x, transform.position.y)) > 1f));
        
        while (Vector2.Distance(transform.position, new Vector2(CurrentTarget.position.x, transform.position.y)) > 1f && 
                   (Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)) > 1f /*Away from player*/  || 
                    (targetIsStair || !targetingPlayer || roomOnly)))
        {
            // Debug.Log("targetIsStair? " + targetIsStair);
            // Debug.Log("Close To Player? " + !(!targetIsStair && Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)) < 1f));
            // Debug.Log("Close To target? " + (Vector2.Distance(transform.position, new Vector2(CurrentTarget.position.x, transform.position.y)) > 1f));
            
            // Do movement
            yield return new WaitForFixedUpdate();
            animator.SetBool("IsWalking", true);
            // Debug.Log("Move" + Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x);
            characterController2D.Move(Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x * Time.fixedDeltaTime, 0, false);
        }

        // Debug.Log("Reached Stairs possibly " + CurrentTarget.TryGetComponent(out stair));
        if (targetIsStair)
        {
            // Debug.LogWarning("Reached stairs");
            transform.position = stair.TargetPosition.position;
            currentFloor = currentFloor == 1 ? 0 : 1; // PlayerMovement.Instance.currentRoom.floor;
            characterController2D.m_Rigidbody2D.velocity = Vector2.zero;

            if (targetingPlayer)
            {
                WalkToPlayer(conversation);
            }
            else
            {
                MoveToRoom(targetedRoom, conversation);
            }
            
            // FindTarget(string.IsNullOrEmpty(conversation));
            //
            // while (Vector2.Distance(transform.position, new Vector2(CurrentTarget.position.x, transform.position.y)) > 1f && 
            //        (Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)) > 1f /*Away from player*/  || 
            //         (targetIsStair || !targetingPlayer || roomOnly)))
            // {
            //     // Do movement
            //     yield return new WaitForFixedUpdate();
            //     // Debug.Log("Move" + Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x);
            //     characterController2D.Move(Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x * Time.fixedDeltaTime, 0, false);
            // }
        }
        else
        {
        
            // Debug.Log("Target Room: " + targetRoom);
            // Debug.Log("Player Room: " + PlayerMovement.Instance.currentRoom);

            if (targetingPlayer && targetRoom != PlayerMovement.Instance.currentRoom && Vector2.Distance(transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y)) > 1f)
            {
                Debug.LogWarning("Restarting WalkToPlayer!!! for " + gameObject.name);
                WalkToPlayer(conversation);
            }
            else
            {
            
                Debug.Log("DONE!!");
                if (!string.IsNullOrEmpty(conversation))
                    DialogueManager.instance.StartConversation(conversation);
                DialogueStart.enabled = true;
            
                animator.SetBool("IsWalking", false);
            }
            
        }
    }

    [Button]
    bool xor(bool left, bool right)
    {
        return (left ^ right);
    }

    [Button]
    private void SetAnimationBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
