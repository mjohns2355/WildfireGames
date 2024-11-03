using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace HappyHouse.FireSystem
{
    public class FireManager : MonoBehaviour
    {
        [InspectorButton("StartFireTesting")]
        public bool startFire = false;
        public Transform fireSpawnPoint;
        public GameObject firePrefab;
        public float fireTimer = 100f;
        public List<FireController> spawnedFires = new List<FireController>();
        public float defaultFireLife = 10f;
        private void Start()
        {
           
        }

        private void Update()
        {
            //while (startFire)
            //{
            //    if(fireTimer > 0)
            //    {
            //        fireTimer -= Time.deltaTime;
            //    }
            //    else
            //    {
            //        startFire = false;
            //    }
            //}
        }
        void StartFire()
        {
            if (startFire) return;
            startFire = true;
            SpawnFire(fireSpawnPoint, 5);

        }


        public void SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false, float life = 0f)
        {
            var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;
            var fireLife =  life == 0? defaultFireLife : life;
            fire.GetComponent<FireController>().InitFire(onCombustible, 2, fireLife);
        }

        // test only function
        public void StartFireSimulation()
        {
            StartFire();
        }
    }
}

