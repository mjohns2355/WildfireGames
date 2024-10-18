using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace HappyHouse.FireSystem
{
    public class FireManager : MonoBehaviour
    {
        bool startFire = false;
        public float fireWaitTimeBeforeStart = 3f;
        public Transform fireSpawnPoint;
        public GameObject firePrefab;

        private void Start()
        {
            StartFire();
        }
        void StartFire()
        {
            if (startFire) return;
            startFire = true;
            SpawnFire(fireSpawnPoint, 1);
        }

        public void SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false)
        {
            var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;
            fire.GetComponent<FireController>().onCombustible = onCombustible;

        }
    }
}

