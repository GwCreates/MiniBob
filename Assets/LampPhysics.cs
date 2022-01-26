using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPhysics : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private float targetAngle = 0;
    [SerializeField] private float rotationForce = 10f;
    
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.centerOfMass = Vector2.zero;
        targetAngle = transform.rotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        rigidbody.AddTorque((targetAngle - rigidbody.rotation) * rotationForce * Time.fixedDeltaTime, ForceMode2D.Force);
    }
}
