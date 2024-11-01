using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHouse.FireSystem
{
    public class FireController : MonoBehaviour
    {
        public bool onCombustible = false;
        //public Combustible combustible;
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
        [SerializeField] Rigidbody rb;
        [SerializeField] BoxCollider collider;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<BoxCollider>();
            scaler = transform.localScale;
           
            if (onCombustible)
            {
                fireSize = maxSize;
                StartCoroutine(OnDestroyFireRoutine());
            }

        }
        private void Update()
        {
            if (!onCombustible)
            {
                rb.velocity = Vector3.left * speed;
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
            
            if (hit.layer == LayerMask.NameToLayer("Structure"))
            {
                // collider is on mesh, need to be replaced later
                if (hit.transform.parent.TryGetComponent(out BaseHousePartObject obj) && obj != null)
                {
                    Debug.Log($"{obj.name} is on fire");
                    obj.Ignite();
                }
            }
        }



        public void ImpactFire(float multiplier)
        {
            var emberVelocity = embers.velocityOverLifetime;
            var fireVelocity = fire.velocityOverLifetime;
            var mediumFlameVelocity = mediumFlame.velocityOverLifetime;
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

            // Burned Logic Here
            Destroy(gameObject);
        }
    }
}

