using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]   
public class ATC_CarController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float torque = 0.5f;
    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private Vector2 movementVector;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 movementInput)
    {
        movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        if(rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * speed);
        }
        rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
    }
}
