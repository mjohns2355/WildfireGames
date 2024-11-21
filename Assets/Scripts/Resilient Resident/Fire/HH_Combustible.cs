using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HappyHouse.FireSystem
{
    public class Combustible : MonoBehaviour
    {
        public Transform fireSpawnPos;
        public float fireChance = 1;
        public MeshRenderer[] meshes;
        [SerializeField] bool isOnfire = false;
        FireMovementController fire;
        [SerializeField] float waitTimeBeforeCatchOnFire;
        public bool burned = false;
        //durability
        private float burnTime = 0;
        //flammability
        private float burnChance = 0;

        [SerializeField] HouseManager houseManager;
        //private ATC_dialogManager dialog;

        // Start is called before the first frame update
        void Start()
        {
            meshes = GetComponentsInChildren<MeshRenderer>();
            houseManager = GetComponentInParent<HouseManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //if (isOnfire && !burned)
            //{
            //    burnTime += Time.deltaTime;
            //    foreach (MeshRenderer m in meshes)
            //    {
            //        m.material.color = Color.Lerp(m.material.color, burntColor, Time.deltaTime);
            //    }
            //    if (burnTime > 30 && !burned && !GameManager.Instance.SimIsEnd)
            //    {
            //        GameManager.Instance.housesDestroyed++;
            //        //dialog.houseDestroyed++;
            //        burned = true;
            //    }
            //}
        }

        public virtual void CatchOnFire()
        {
            if (isOnfire || burned) return;
            //if (fire != null && fire.isInFireSafeZone) return;
            if (Random.Range(0.4f, 1) > fireChance)
            {
                fireChance += Time.deltaTime;
                return;
            }
            if (GameManager.Instance.SimIsEnd) return;
            StartCoroutine(CatchOnFireRoutine());

        }

        public virtual IEnumerator CatchOnFireRoutine()
        {
            yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
           

        }



    }
}

