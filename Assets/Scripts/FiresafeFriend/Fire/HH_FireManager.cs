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
        public GameObject mainEmbersPrefab;
        public GameObject backgroundFire;
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
                backgroundFire.SetActive(false);
                fireEndEvent.Invoke();
            }
        }
        void StartFire()
        {
            if (startFire) return;
            startFire = true;
            mainFire = SpawnEmbers(fireSpawnPoint, 50);
            backgroundFire.SetActive(true);
        }

        public FireController SpawnEmbers(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false, float life = 0f)
        {
            var fire = Instantiate(mainEmbersPrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;
            var fireLife = life == 0 ? defaultFireLife : life;
            fire.GetComponent<FireController>().InitFire(onCombustible, 2, fireLife,10f);
            return fire.GetComponent<FireController>();
        }
        public FireController SpawnFire(Vector3 spawnPos, Transform spawnParent, float fireSpeed = 1, float scaleMultiplier = 1, bool onCombustible = false, float life = 0f, float maxSize = 1f)
        {
            var fire = Instantiate(firePrefab, spawnPos, Quaternion.identity, spawnParent);
            fire.transform.localScale *= scaleMultiplier;
            
            var fireLife =  life == 0? defaultFireLife : life;
            fire.GetComponent<FireController>().InitFire(onCombustible, fireSpeed, fireLife,maxSize);
            return fire.GetComponent<FireController>();
        }

        // test only function
        public void StartFireSimulation()
        {
            StartFire();
        }
    }
}

