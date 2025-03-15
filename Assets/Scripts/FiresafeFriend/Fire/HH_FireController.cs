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
        public GameObject flame;
        public ParticleSystem embers;
        //public ParticleSystem mediumFlame;
        [Range(0f, 10f)]
        //public float fireGrowthSpeed = 0.2f;
        public float speed;
        public Vector3 windDirection;
        [SerializeField] GameObject fireSFX;
        [SerializeField] float maxSize;
        //[SerializeField] float minSize;
        //[SerializeField] GameObject particleParent;
        //float fireSize = 0;
        [SerializeField] Rigidbody rb;
        //[SerializeField] SphereCollider collider;
        public float fireLife = 10f;
        GameObject SFX;
        public bool canMove = false;
        public LayerMask combustibleLayer;
        public Vector3 startPos;
        public Vector3 endPos;
        //[SerializeField]float totalHeat = 100f;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            //collider = GetComponent<SphereCollider>();
            scaler = transform.localScale;


        }
        private void Update()
        {
            if (!onCombustible)
            {
                rb.velocity = -1f * speed * Vector3.forward;
                ImpactFire(10);
            }

            if (fireLife > 0 && onCombustible)
            {
                fireLife -= Time.deltaTime;
                GraduallyChangeFireSize(maxSize, 0.1f);
            }
            if (fireLife < 0)
            {
                Destroy(gameObject);

            }
            if (!canMove) return;
            transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        }



        public void InitFire(bool isOnCombustible, float life, float maxSize)
        {
            onCombustible = isOnCombustible;
            //this.speed = speed;
            fireLife = life == 0 ? fireLife : life;
            this.maxSize = maxSize;

            if (!onCombustible)
            {
                SFX = Instantiate(fireSFX, transform);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            //if (!onCombustible) return;
            //var hit = other.gameObject;
            //if (hit == null) return;
            //if (gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer("Nature"))
            //{
            //    onVegetation = true;
            //}
            ////only use collision for burned vegetation spreading fire to house part
            //if (hit.layer == LayerMask.NameToLayer("Structure") && onVegetation)
            //{

            //    // collider is on mesh
            //    if (hit.transform.parent.TryGetComponent(out FF_Combustible obj) && obj != null)
            //    {
            //        Debug.Log($"Vegetation Spread Fire to {obj.name}");
            //        obj.TryIgnite();
            //    }
            //}

            //if (hit.layer == LayerMask.NameToLayer("Nature"))
            //{
            //    // collider is on mesh
            //    if (hit.transform.TryGetComponent(out FF_Combustible obj) && obj != null)
            //    {

            //        obj.TryIgnite();
            //    }
            //}
        }

        private void OnTriggerStay(Collider other)
        {
            if (onCombustible/* || totalHeat <= 0*/) return;

            var hit = other.gameObject;
            if (hit == null) return;

            //if (hit.layer == LayerMask.NameToLayer("Structure") || hit.layer == LayerMask.NameToLayer("Nature") || hit.layer == LayerMask.NameToLayer("Combustible"))
            //{
            // collider is on mesh
            FF_BaseCombustible obj;
            //Debug.Log($"Hit {hit.name}");
            if (hit.layer == LayerMask.NameToLayer("Structure"))
            {
                hit.transform.parent.TryGetComponent(out obj);

            }
            else
            {
                hit.transform.TryGetComponent(out obj);
            }

            if (obj != null)
            {
                obj.AddHeat(0.5f);
            }
        }

        public void ImpactFire(float multiplier)
        {
            var emberVelocity = embers.velocityOverLifetime;
            //var fireVelocity = fire.velocityOverLifetime;
            //var mediumFlameVelocity = mediumFlame.velocityOverLifetime;
            emberVelocity.xMultiplier = windDirection.x * multiplier;
            emberVelocity.zMultiplier = windDirection.z * multiplier;
            var emberSize = embers.sizeOverLifetime;
            //emberSize.sizeMultiplier = multiplier;
           
        }

        public void GraduallyChangeFireSize(float targetSize, float t)
        {
            transform.localScale = scaler;
            scaler.x = Mathf.Lerp(scaler.x, targetSize, t * Time.deltaTime);
            scaler.y = Mathf.Lerp(scaler.y, targetSize, t * Time.deltaTime);
            scaler.z = Mathf.Lerp(scaler.z, targetSize, t * Time.deltaTime);
        }

        //IEnumerator ChangeFireSizeRoutine(float maxSize)
        //{
        //    yield return new WaitForSeconds(waitTime);
        //    GraduallyChangeFireSize(maxSize, 0.02f);
        //}

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

