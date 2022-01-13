using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(CinemachineVirtualCamera))]
public class CameraTrigger : MonoBehaviour
{
    public static CameraTrigger CurrentlyActiveCamera;
    public static CameraTrigger PreviouslyActiveCamera;

    [SerializeField, Range(0, 100)] private int priority = 100;

    [NonSerialized] public CinemachineVirtualCamera VirtualCamera;

    [SerializeField] private bool activateOnStart = false;
    
    void Awake()
    {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (!activateOnStart)
            VirtualCamera.Priority = 0;
    }

    private void Start()
    {
        if (activateOnStart)
        {
            TriggerCamera();
        }
    }

    protected virtual void TriggerCamera()
    {
        VirtualCamera.Priority = priority;
        if (CurrentlyActiveCamera != null)
            CurrentlyActiveCamera.VirtualCamera.Priority = 0;
        PreviouslyActiveCamera = CurrentlyActiveCamera;
        CurrentlyActiveCamera = this;
        CurrentlyActiveCamera.VirtualCamera.MoveToTopOfPrioritySubqueue();
    }

    protected virtual void RevertCamera()
    {
        VirtualCamera.Priority = 0;
        if (PreviouslyActiveCamera != null)
            PreviouslyActiveCamera.VirtualCamera.Priority = PreviouslyActiveCamera.priority;
        CurrentlyActiveCamera = PreviouslyActiveCamera;
        PreviouslyActiveCamera = this;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CurrentlyActiveCamera != this && other.CompareTag("Player"))
        {
            TriggerCamera();
        }
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CurrentlyActiveCamera == this && other.CompareTag("Player"))
        {
            RevertCamera();
        }
    }
}
