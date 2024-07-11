using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustible : MonoBehaviour
{
    public Transform fireSpawnPos;
    public float fireChance = 1;
    float waitTimeBeforeCatchOnFire;
    [SerializeField]bool isOnfire = false;
    FireMovementController fire;

    // Start is called before the first frame update
    void Start()
    {
        waitTimeBeforeCatchOnFire = Random.Range(0.5f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CatchOnFire()
    {
        if (isOnfire) return;
        StartCoroutine(CatchOnFireRoutine());
        
    }

    IEnumerator CatchOnFireRoutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
        GameManager.Instance.fireManager.SpawnFire(fireSpawnPos, 0.3f, true);
        isOnfire = true;
        fire = fireSpawnPos.GetComponentInChildren<FireMovementController>();
    }
}
