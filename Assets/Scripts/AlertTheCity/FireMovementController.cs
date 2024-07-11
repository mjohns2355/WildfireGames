using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMovementController : MonoBehaviour
{
    public bool onCombustible = false;
    public float waitTime = 1f;
    public Vector3 scaler;
    public ParticleSystem fire;
    public ParticleSystem embers;
    public ParticleSystem mediumFlame;
    public float speed;
    public Vector3 windDirection;
    //ParticleSystem.VelocityOverLifetimeModule emberVelocity;
    //ParticleSystem.VelocityOverLifetimeModule fireVelocity;
    //ParticleSystem.VelocityOverLifetimeModule mediumFlameVelocity;
    [SerializeField]Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        scaler = transform.localScale;


    }
    private void Update()
    {
        if (!onCombustible)
        {
            rb.velocity = windDirection * speed;
        }

    }

    private void FixedUpdate()
    {
        if (onCombustible)
        {
            StartCoroutine(IncreaseFireSizeRoutine(2f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        if (hit!=null && hit.layer == LayerMask.NameToLayer("Nature") || hit.layer == LayerMask.NameToLayer("Structure"))
        {
            Debug.Log("Fire collides with: " + other.name);
            gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            Combustible combustible;
            hit.TryGetComponent(out combustible);
            if (combustible != null)
            {
                combustible.CatchOnFire();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wind"))
        {
            Debug.Log("Fire left: " + other.name);
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

    public void GraduallyIncreaseFireSize(float maxSize)
    {
        transform.localScale = scaler;
        scaler.x = Mathf.Lerp(scaler.x, maxSize, .03f * Time.deltaTime);
        scaler.y = Mathf.Lerp(scaler.y, maxSize, .03f * Time.deltaTime);
        scaler.z = Mathf.Lerp(scaler.z, maxSize, .03f * Time.deltaTime);
    }

    IEnumerator IncreaseFireSizeRoutine(float maxSize)
    {
        yield return new WaitForSeconds(waitTime);
        GraduallyIncreaseFireSize(maxSize);
    }
}
