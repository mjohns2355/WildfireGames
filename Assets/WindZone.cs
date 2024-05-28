using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : UnitySingleton<WindZone>
{
    [Range(0f, 10f)]
    [SerializeField] float windForce = 0f;
    [Range(0f,10f)]
    [SerializeField] float windSpeed = 0f;
    [Range(0f,100f)]
    [SerializeField] float range = 10f;
    [InspectorButton("ChangeWindRange")]
    public bool Change;
    public bool isStill = false;
    [SerializeField] Vector3 windDirection;
 
    Rigidbody rb;
    BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        windDirection = transform.right;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        transform.localScale = new Vector3(range, 8, range);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStill)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        rb.velocity = windSpeed * windDirection;
    }

    private void FixedUpdate()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        var hit = other.gameObject;
        if (hit != null && hit.layer == LayerMask.NameToLayer("Fire"))
        {
            var fire = hit.GetComponent<FireMovementController>();
            fire.windDirection = windDirection;
            fire.speed = windForce;
            fire.ImpactFire(windForce);
        }
    }

    private void ChangeWindRange()
    {
        transform.localScale = new Vector3(range, 8,  range);
    }
}
