using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarSpeed { slow, medium, fast };
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;
    
    public CarSpeed carSpeed = CarSpeed.medium;
    [SerializeField]
    private float power = 5;
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
    private void Start()
    {
        switch (carSpeed)
        {
            case CarSpeed.slow:
                power = 10; break;
            case CarSpeed.medium:
                power = 20; break;
            case CarSpeed.fast:
                power = 30; break;
        }
    }
    public void Move(Vector2 movementInput)
    {
        this.movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        if(rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * power);
        }
        rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
    }

}
