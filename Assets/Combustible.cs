using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustible : MonoBehaviour
{
    public Transform fireSpawnPos;
    public float fireChance = 1;
    bool isOnfire = false;
    FireMovementController fire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CatchOnFire()
    {
        if (isOnfire) return;
        GameManager.Instance.fireManager.SpawnFire(fireSpawnPos,0.3f,true);
        isOnfire = true;
        fire = fireSpawnPos.GetComponentInChildren<FireMovementController>();
    }
}
