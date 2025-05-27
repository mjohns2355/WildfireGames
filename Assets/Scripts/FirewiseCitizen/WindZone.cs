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
    public Vector3 WindDirection {  get; private set; }

    float windDirChangeInterval = 10f;
    float windTimer = 0f;
    float speedMultiplier;
    Rigidbody rb;
    BoxCollider collider;
    public GameObject fireSFX;
    // Start is called before the first frame update
    void Start()
    {
        speedMultiplier = GameManager.Instance.SimulationSpeed;
        WindDirection = transform.right;
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
        //else
        //{
        //    if (!fireSFX.activeInHierarchy)
        //    {
        //        fireSFX.SetActive(true);
        //    }
        //}
        windTimer += Time.deltaTime;
        if (windTimer>= windDirChangeInterval/speedMultiplier)
        {
            //Debug.Log("Change Wind Direction");
            RandomizeWindDirection();
            windTimer = 0f;
        }
        rb.velocity = windSpeed * WindDirection * speedMultiplier ;
    
}

    private void OnTriggerStay(Collider other)
    {
        var hit = other.gameObject;
        if (hit != null && hit.layer == LayerMask.NameToLayer("Fire"))
        {
            
            var fire = hit.GetComponent<FireMovementController>();
            fire.windDirection = WindDirection;
            fire.speed = windSpeed * speedMultiplier;
            fire.ImpactFire(windForce);
        }
    }
    public void SetWindDirection(Vector3 direction)
    {
        WindDirection = direction;
    }
    void RandomizeWindDirection() {
        float maxAngleChange = 30f;
        float randomAngle = Random.Range(-maxAngleChange, maxAngleChange);

        // Change direction
        Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
        Vector3 newDirection = rotation * WindDirection;

        WindDirection = newDirection.normalized;

        //windSpeed = Random.Range(3f, 10f);

        //Debug.Log($"Wind direction: {WindDirection}, Wind speed: {windSpeed}");
        //float randomX = Random.Range(-1f, 1f);
        //float randomZ = Random.Range(-1f, 1f);

        //Vector3 randomDirection = new Vector3(randomX, 0, randomZ);
        //WindDirection = randomDirection;
        //transform.rotation = Random.Range
    }

    private void ChangeWindRange()
    {
        transform.localScale = new Vector3(range, 8,  range);
    }


}
