using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;
    
    public CarSpeed carSpeed = CarSpeed.medium;
    public ATC_StructureModel start;
    public ATC_StructureModel end;

    [SerializeField]
    private float power = 5;
    [SerializeField]
    private float torque = 0.5f;
    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private Vector2 movementVector;
    //[SerializeField]
    //float currentVelocity;
    //float waitTime = 3f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        //StartCoroutine(CheckIfCarIsMoving());
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
        movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        if(rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * power);
        }
        rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
    }

    //IEnumerator CheckIfCarIsMoving()
    //{
    //    bool state = true;
    //    while (state)
    //    {
    //        yield return new WaitForSeconds(waitTime);
    //        if (rb.velocity.magnitude == 0)
    //        {
    //            Debug.Log("Car stopped");
    //            ATC_AIDirector.Instance.RespawnACar(start,end,carSpeed);
    //            state = false;
    //            Destroy(gameObject);
    //            break;
    //        }
    //    }
    //}
}
