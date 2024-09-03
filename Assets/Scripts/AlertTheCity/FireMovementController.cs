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
    float fireSize = 0;
    //ParticleSystem.VelocityOverLifetimeModule emberVelocity;
    //ParticleSystem.VelocityOverLifetimeModule fireVelocity;
    //ParticleSystem.VelocityOverLifetimeModule mediumFlameVelocity;
    [SerializeField]Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        scaler = transform.localScale;
        waitTime = Random.Range(3f, 5f);
        if(onCombustible)
        {
            fireSize = maxSize;
            StartCoroutine(OnDestroyFireRoutine());
        }
        //else
        //{
        //    fireSize = 1;
        //}

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
        if (hit!=null && hit.layer == LayerMask.NameToLayer("Nature") || hit.layer == LayerMask.NameToLayer("Structure"))
        {

            //Debug.Log("Fire collides with: " + other.name);
            //gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            Combustible obj;
            hit.TryGetComponent(out obj);
            if (obj != null)
            {
                obj.CatchOnFire();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wind"))
        {
            //Debug.Log("Fire left: " + other.name);
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

    public void GraduallyChangeFireSize(float maxSize)
    {
        transform.localScale = scaler;
        scaler.x = Mathf.Lerp(scaler.x, maxSize, fireGrowthSpeed * Time.deltaTime);
        scaler.y = Mathf.Lerp(scaler.y, maxSize, fireGrowthSpeed * Time.deltaTime);
        scaler.z = Mathf.Lerp(scaler.z, maxSize, fireGrowthSpeed * Time.deltaTime);
    }

    IEnumerator ChangeFireSizeRoutine(float maxSize)
    {
        yield return new WaitForSeconds(waitTime);
        GraduallyChangeFireSize(maxSize);
    }

    IEnumerator OnDestroyFireRoutine()
    {
        yield return new WaitForSeconds(30f);
        //Debug.Log("Fire start to shrink");
        fireSize = minSize;
        yield return new WaitForSeconds(10f);
        //Debug.Log("Destroy Fire");
        if(combustible != null && combustible.gameObject.layer != 7 && combustible.burned)
        {

            Instantiate(Resources.Load("Burned"), combustible.transform.position, combustible.transform.rotation, combustible.transform.parent);
            Destroy(combustible.gameObject);
        }
        Destroy(gameObject);
    }
}
