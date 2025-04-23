using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMovementController : MonoBehaviour
{
    public bool onCombustible = false;
    public Combustible combustible;
    public float waitTime = 1f;
    public Vector3 scaler;
    public ParticleSystem fire;
    public ParticleSystem embers;
    public ParticleSystem mediumFlame;
    [Range(0f, 1f)]
    public float fireGrowthSpeed = 0.2f;
    public float speed;
    public Vector3 windDirection;
    [SerializeField] float maxSize;
    [SerializeField] float minSize;
    [SerializeField] GameObject particleParent;
    float fireSize = 0;
    //ParticleSystem.VelocityOverLifetimeModule emberVelocity;
    //ParticleSystem.VelocityOverLifetimeModule fireVelocity;
    //ParticleSystem.VelocityOverLifetimeModule mediumFlameVelocity;
    [SerializeField]Rigidbody rb;
    [SerializeField] BoxCollider collider;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        scaler = transform.localScale;
        waitTime = Random.Range(3f, 5f);
        if(onCombustible)
        {
            fireSize = maxSize;
            StartCoroutine(OnDestroyFireRoutine());
        }


    }
    private void Update()
    {
        if (!onCombustible)
        {
            rb.velocity = windDirection * speed ;
        }


    }

    private void FixedUpdate()
    {

        if (onCombustible)
        {
            StartCoroutine(ChangeFireSizeRoutine(fireSize));
        }
        else
        {
            StartCoroutine(ChangeFireSizeRoutine(1f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        if (hit == null) return;
        // Check for fire-safe zone first
        if (hit.layer == LayerMask.NameToLayer("FireSafe") && !onCombustible)
        {

            particleParent.SetActive(false);
            return;
        }

        if (hit.layer == LayerMask.NameToLayer("Nature") || hit.layer == LayerMask.NameToLayer("Structure") || hit.layer == LayerMask.NameToLayer("Car"))
        {
            Combustible obj;
            if (hit.TryGetComponent(out obj) && obj != null)
            {
                //Debug.Log($"Fire: {gameObject.GetInstanceID()} on combustible spread fire to another combustible");
                obj.CatchOnFire();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hit = other.gameObject;
        if (hit != null && hit.layer == LayerMask.NameToLayer("FireSafe"))
        {
            //isInFireSafeZone = false;
            //Debug.Log(gameObject.GetInstanceID() + " Left fire safe zone");
            particleParent.SetActive(true);
            //GraduallyChangeFireSize(maxSize, 10f);
        }
        if (hit != null && hit.layer == LayerMask.NameToLayer("Wind") /*&& !isInFireSafeZone*/)
        {
            speed = 1;
            ImpactFire(1);
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }

    public void ImpactFire(float multiplier)
    {
        var emberVelocity = embers.velocityOverLifetime;
        var fireVelocity = fire.velocityOverLifetime;
        var mediumFlameVelocity = mediumFlame.velocityOverLifetime;
        //emberVelocity.x = windDirection.x;
        //emberVelocity.z = windDirection.z;
        emberVelocity.xMultiplier = windDirection.x * multiplier;
        emberVelocity.zMultiplier = windDirection.z * multiplier;
        var emberSize = embers.sizeOverLifetime;
        emberSize.sizeMultiplier = multiplier;
    }

    public void GraduallyChangeFireSize(float maxSize, float t)
    {
        transform.localScale = scaler;
        scaler.x = Mathf.Lerp(scaler.x, maxSize, t * Time.deltaTime);
        scaler.y = Mathf.Lerp(scaler.y, maxSize, t * Time.deltaTime);
        scaler.z = Mathf.Lerp(scaler.z, maxSize, t * Time.deltaTime);
    }

    IEnumerator ChangeFireSizeRoutine(float maxSize)
    {
        yield return new WaitForSeconds(waitTime);
        GraduallyChangeFireSize(maxSize, fireGrowthSpeed);
    }

    IEnumerator OnDestroyFireRoutine()
    {
        yield return new WaitForSeconds(30f);
        //Debug.Log("Fire start to shrink");
        fireSize = minSize;
        yield return new WaitForSeconds(10f);
        //Debug.Log("Destroy Fire");
        if(combustible != null && combustible.burned)
        {
            switch(combustible.gameObject.layer)
            {
                case 10:
                    //Debug.Log("Burned Structure");
                    Instantiate(Resources.Load("Burned"), combustible.transform.position, combustible.transform.rotation, combustible.transform.parent);
                    Destroy(combustible.gameObject);
                    break;
                case 9:
                    //Debug.Log("Burned Car");
                    Destroy(combustible.transform.parent.gameObject);
                    break;

            }
            
        }
        Destroy(gameObject);
    }
}
