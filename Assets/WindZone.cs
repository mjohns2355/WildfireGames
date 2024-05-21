using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : UnitySingleton<WindZone>
{
    public float windForce = 0f;
    public float windSpeed = 0f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = windSpeed * transform.right;
    }

    private void OnTriggerStay(Collider other)
    {
        var hit = other.gameObject;
        if (hit != null && hit.layer == LayerMask.NameToLayer("Fire"))
        {
            var fire = hit.GetComponent<FireMovementController>();
            fire.windDirection = transform.right;
            fire.speed = windForce;
            fire.ImpactFire(windForce);
        }
    }

}
