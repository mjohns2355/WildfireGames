using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public bool startFire = false;
    public Transform fireSpawnPoint;
    public GameObject firePrefab;
    //public List<FireMovementController> fireList;
    // Start is called before the first frame update
    void Start()
    {
        if (!startFire) return;
        SpawnFire(fireSpawnPoint,3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnFire(Transform spawnPos, float scaleMultiplier = 1, bool onCombustible = false)
    {

        var fire = Instantiate(firePrefab, spawnPos.position, Quaternion.identity, spawnPos);
        fire.transform.localScale *= scaleMultiplier;

        fire.GetComponent<FireMovementController>().onCombustible = onCombustible;
    }
}
