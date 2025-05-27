using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;
    
    public CarSpeed carSpeed = CarSpeed.medium;
    public ATC_StructureModel start;
    public List<ATC_StructureModel> ends = new List<ATC_StructureModel>();

    [SerializeField]
    private float power = 5;
    [SerializeField]
    private float torque = 0.5f;
    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private Vector2 movementVector;
    private float speedMultiplier;
    float scaledPower;
    float scaledTorque;
    float scaledMaxSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       
    }
    private void Start()
    {
        speedMultiplier = GameManager.Instance.SimulationSpeed;
        
        float simPow = Mathf.Sqrt(speedMultiplier);
        switch (carSpeed)
        {
            case CarSpeed.slow:
                power = 10; break;
            case CarSpeed.medium:
                power = 20; break;
            case CarSpeed.fast:
                power = 30; break;
        }
        scaledPower = power * speedMultiplier;
        scaledTorque = torque * speedMultiplier;
        scaledMaxSpeed = maxSpeed * simPow;
    }
    public void Move(Vector2 movementInput)
    {
        movementVector = movementInput;
    }

    private void FixedUpdate()
    {
       
        if (movementVector.sqrMagnitude < 0.001f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (rb.velocity.magnitude < scaledMaxSpeed)
            rb.AddForce(movementVector.y * transform.forward * scaledPower,
                        ForceMode.Force);

        rb.AddTorque(movementVector.x * Vector3.up * scaledTorque,
                     ForceMode.Force);

        if (rb.velocity.sqrMagnitude > scaledMaxSpeed * scaledMaxSpeed)
            rb.velocity = rb.velocity.normalized * scaledMaxSpeed;


    }

}
