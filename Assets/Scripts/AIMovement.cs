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
    
    // Start is called before the first frame update
    void Start()
    {
        
        characterController2D = GetComponent<CharacterController2D>();
        Lua.RegisterFunction("Move" + name + "ToPlayer", this, SymbolExtensions.GetMethodInfo(() => WalkToPlayer(string.Empty)));
        Lua.RegisterFunction("Move" + name + "ToRoom", this, SymbolExtensions.GetMethodInfo(() => MoveToRoom(string.Empty)));
    }

    void Update()
    {
        if (shouldWalkAfterConversation && !DialogueManager.IsConversationActive)
        {
            WalkToPlayer(nextConversation);
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
        if (!string.IsNullOrEmpty(room))
        {
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

                StartCoroutine(WalkToPlayerCoroutine(""));
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
            FindTarget();

            StartCoroutine(WalkToPlayerCoroutine(conversation));
        }
    }

    private void FindTarget()
    {
        if (PlayerMovement.Instance.currentRoom.floor != currentFloor)
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
            CurrentTarget = female ? PlayerMovement.Instance.currentRoom.targetPositionFemale : PlayerMovement.Instance.currentRoom.targetPositionMale;
        }
    }

    private IEnumerator WalkToPlayerCoroutine(string conversation)
    {
        Vector2 targetPosition = CurrentTarget.transform.position;
        targetPosition.y = transform.position.y;
        Debug.Log("Distance: " + Vector2.Distance(transform.position, targetPosition));
        while (Vector2.Distance(transform.position, targetPosition) > 1f)
        {
            // Do movement
            yield return new WaitForFixedUpdate();
            Debug.Log("Move" + Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x);
            characterController2D.Move(Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x * Time.fixedDeltaTime, 0, false);
        }

        Stairs stair;
        Debug.Log("REached Stairs possibly " + CurrentTarget.TryGetComponent(out stair));
        if (CurrentTarget.TryGetComponent(out stair))
        {
            Debug.LogWarning("Reached stairs");
            transform.position = stair.TargetPosition.position;
            currentFloor = PlayerMovement.Instance.currentRoom.floor;
            characterController2D.m_Rigidbody2D.velocity = Vector2.zero;
            FindTarget();
            
            targetPosition = CurrentTarget.transform.position;
            targetPosition.y = transform.position.y;
            while (Vector2.Distance(transform.position, targetPosition) > 1f)
            {
                // Do movement
                yield return new WaitForFixedUpdate();
                Debug.Log("Move" + Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x);
                characterController2D.Move(Mathf.Clamp(CurrentTarget.transform.position.x - transform.position.x, -1f, 1f) * movementSpeed.x * Time.fixedDeltaTime, 0, false);
            }
        }
        
        Debug.Log("DONE!!");
        if (!string.IsNullOrEmpty(conversation))
            DialogueManager.instance.StartConversation(conversation);
        DialogueStart.enabled = true;
    }
}
