using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_WindZone : MonoBehaviour
{
    [Range(0f, 10f)]
    [SerializeField] float windForce = 0f;
    [Range(0f,10f)]
    [SerializeField] float windSpeed = 0f;
    [Range(0f,100f)]
    [SerializeField] float range = 10f;
    [InspectorButton("ChangeWindRange")]
    public bool Change;
    public bool isStill = true;
    [SerializeField] Vector3 windDirection;

    float windDirChangeInterval = 20f;
    float windTimer = 0f;
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
        windTimer += Time.deltaTime;
        if (windTimer>= windDirChangeInterval)
        {
            Debug.Log("Change Wind Direction");
            RandomizeWindDirection();
            windTimer = 0f;
        }
        rb.velocity = windSpeed * windDirection ;
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
            fire.speed = windSpeed;
            fire.ImpactFire(windForce);
        }
    }

    void RandomizeWindDirection() {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        Vector3 randomDirection = new Vector3(randomX, 0, randomZ);
        windDirection = randomDirection;
        //transform.rotation = Random.Range
    }

    private void ChangeWindRange()
    {
        transform.localScale = new Vector3(range, 8,  range);
    }


}
