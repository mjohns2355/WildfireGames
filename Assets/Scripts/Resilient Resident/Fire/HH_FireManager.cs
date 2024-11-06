using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
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
        public UnityEvent fireEndEvent;
        FireController mainFire;
        private void Start()
        {
           
        }

        private void Update()
        {

            if(!startFire) return;
            if (fireTimer > 0)
            {
                fireTimer -= Time.deltaTime;
            }
            else
            {
                startFire = false;
                Destroy(mainFire.gameObject);
                fireEndEvent.Invoke();
            }
        }
        void StartFire()
        {
            if (startFire) return;
            startFire = true;
            mainFire = SpawnFire(fireSpawnPoint, 3);

        }


        public FireController SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false, float life = 0f)
        {
            var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;
            var fireLife =  life == 0? defaultFireLife : life;
            fire.GetComponent<FireController>().InitFire(onCombustible, 5, fireLife);
            return fire.GetComponent<FireController>();
        }

        // test only function
        public void StartFireSimulation()
        {
            StartFire();
        }
    }
}

