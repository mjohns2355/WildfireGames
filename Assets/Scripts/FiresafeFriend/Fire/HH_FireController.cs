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
        [SerializeField] GameObject fireSFX;
        [SerializeField] float maxSize;
        [SerializeField] float minSize;
        [SerializeField] GameObject particleParent;
        float fireSize = 0;
        [SerializeField] Rigidbody rb;
        [SerializeField] SphereCollider collider;
        float fireLife;
        GameObject SFX;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<SphereCollider>();
            scaler = transform.localScale;
           
            if (onCombustible)
            {
                //StartCoroutine(OnDestroyFireRoutine());
            }

        }
        private void Update()
        {
            if (!onCombustible)
            {
                rb.velocity = Vector3.left * speed;
                ImpactFire(10);
            }

            while (fireLife > 0 && onCombustible)
            {
                fireLife -= Time.deltaTime;
                GraduallyChangeFireSize(0.01f, fireGrowthSpeed);
            }
            

        }

        public void InitFire(bool isOnCombustible, float speed, float life)
        {
            onCombustible = isOnCombustible;
            this.speed = speed;
            fireLife = life;
            if (!onCombustible)
            {
                SFX = Instantiate(fireSFX, transform);
            }
           
        }


        private void OnTriggerEnter(Collider other)
        {
            var hit = other.gameObject;
            if (hit == null) return;
            
            if (hit.layer == LayerMask.NameToLayer("Structure"))
            {
               // collider is on mesh
                if (hit.transform.parent.TryGetComponent(out FF_Combustible obj) && obj != null)
                {
                   
                    obj.TryIgnite();
                }
            }

            if (hit.layer == LayerMask.NameToLayer("Nature"))
            {
                // collider is on mesh
                if (hit.transform.TryGetComponent(out FF_Combustible obj) && obj != null)
                {

                    obj.TryIgnite();
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

        public void GraduallyChangeFireSize(float targetSize, float t)
        {
            transform.localScale = scaler;
            scaler.x = Mathf.Lerp(scaler.x, targetSize, t * Time.deltaTime);
            scaler.y = Mathf.Lerp(scaler.y, targetSize, t * Time.deltaTime);
            scaler.z = Mathf.Lerp(scaler.z, targetSize, t * Time.deltaTime);
        }

        IEnumerator ChangeFireSizeRoutine(float maxSize)
        {
            yield return new WaitForSeconds(waitTime);
            GraduallyChangeFireSize(maxSize, fireGrowthSpeed);
        }

        //IEnumerator OnDestroyFireRoutine()
        //{
        //    Debug.Log($"Fire life: {fireLife}");
        //    yield return new WaitForSeconds(fireLife);
        //    //Debug.Log("Fire start to shrink");
           
        //    yield return new WaitForSeconds(10f);
        //    //Debug.Log("Destroy Fire");

        //    // Burned Logic Here
        //    Destroy(gameObject);
        //}
    }
}

