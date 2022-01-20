using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public string name = "Female";

    public bool female = true;
    public Transform CurrentTarget;
    public int currentFloor = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
        characterController2D = GetComponent<CharacterController2D>();
        Lua.RegisterFunction("Move" + name + "ToPlayer", this, SymbolExtensions.GetMethodInfo(() => WalkToPlayer(string.Empty)));
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("Move" + name + "ToPlayer");
    }

    private CharacterController2D characterController2D;

    [SerializeField] private Vector2 movementSpeed = new Vector2(25f, 1f);
    

    [Button]
    void WalkToPlayer(string conversation)
    {
        FindTarget();

        StartCoroutine(WalkToPlayerCoroutine(conversation));
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
    }
}
