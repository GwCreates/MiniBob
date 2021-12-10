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
    
    void Awake()
    {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        VirtualCamera.Priority = 0;
    }

    protected virtual void TriggerCamera()
    {
        VirtualCamera.Priority = priority;
        if (CurrentlyActiveCamera != null)
            CurrentlyActiveCamera.VirtualCamera.Priority = 0;
        PreviouslyActiveCamera = CurrentlyActiveCamera;
        CurrentlyActiveCamera = this;
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
