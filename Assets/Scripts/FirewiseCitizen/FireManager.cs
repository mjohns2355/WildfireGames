using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    bool startFire = false;
    public float fireWaitTimeBeforeStart = 3f;
    //public Transform fireSpawnPoint;
    public GameObject firePrefab;
    public ATC_WindZone wind;
    //public GameObject fireSafeZone;
    public Transform townCenter;
    //public List<FireMovementController> fireList;
    // Start is called before the first frame update
    public bool done = false;
    public Transform fireStartPoints;
    private List<Transform> fireStartPositions = new List<Transform>();
    void Start()
    {
        GameManager.Instance.SimStartsEvent.AddListener(() => 
        {
            StartCoroutine(StartFireRoutine());
            wind.isStill= false;
        });

        GameManager.Instance.SimEndsEvent.AddListener(() =>
        {
            done = true;
            wind.isStill= true;
            StopFireEffect();
        });

        //fireSafeZone.GetComponent<MeshRenderer>().enabled = false;
        var fireSafeZones = GameObject.FindGameObjectsWithTag("FireSafeZone");
        foreach(var zone in fireSafeZones)
        {
            zone.GetComponent<MeshRenderer>().enabled = false;
        }

        for (int i = 0; i < fireStartPoints.childCount; i++)
        {
            fireStartPositions.Add(fireStartPoints.GetChild(i));
        }
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
        var fireSpawnPoint = fireStartPositions.OrderBy(x => Random.value).First();
        wind.transform.position = fireSpawnPoint.position;
        var windDirection = (townCenter.position - fireSpawnPoint.position).normalized;
        //Debug.Log(windDirection);
        wind.SetWindDirection(windDirection);
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

    void StopFireEffect()
    {
        GameObject[] fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject f in fires)
        {
            Destroy(f);
        }
    }

    public void HideFireParticle()
    {
        GameObject[] fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject f in fires)
        {
            f.GetComponent<FireMovementController>().ToggleParticleStatus(false);
        }
    }
}
