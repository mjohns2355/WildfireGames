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

        private void Start()
        {
           
        }
        void StartFire()
        {
            if (startFire) return;
            startFire = true;
            SpawnFire(fireSpawnPoint, 2);
        }

        public void SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false)
        {
            var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;
            fire.GetComponent<FireController>().onCombustible = onCombustible;
            fire.GetComponent<FireController>().speed = 2;
        }

        // test only function
        public void StartFireTesting()
        {
            StartFire();
        }
    }
}

