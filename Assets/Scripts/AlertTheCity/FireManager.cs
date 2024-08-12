using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    bool startFire = false;
    public float fireWaitTimeBeforeStart = 3f;
    public Transform fireSpawnPoint;
    public GameObject firePrefab;
    //public List<FireMovementController> fireList;
    // Start is called before the first frame update
    public bool done = false;

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartFireRoutine()
    {
        yield return new WaitForSeconds(fireWaitTimeBeforeStart);
        StartFire();
    }
    void StartFire()
    {
        if(startFire) return;
        startFire = true;
        SpawnFire(fireSpawnPoint, 1);
    }
    public void SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false)
    {
        if (!done)
        {

            var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
            fire.transform.localScale *= scaleMultiplier;

            fire.GetComponent<FireMovementController>().onCombustible = onCombustible;
        }
    }
}
