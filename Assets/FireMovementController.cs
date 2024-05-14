using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMovementController : MonoBehaviour
{
    public ParticleSystem fire;
    public float speed;
    public Vector3 direction;
    
    [SerializeField]Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
