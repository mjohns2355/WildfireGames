using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Combustible : MonoBehaviour
{
    public Transform fireSpawnPos;
    public float fireChance = 1;
    public List<MeshRenderer> meshes = new List<MeshRenderer>();
    [SerializeField]bool isOnfire = false;
    [SerializeField] Color burntColor;
    FireMovementController fire;
    [SerializeField] float waitTimeBeforeCatchOnFire;
    // Start is called before the first frame update
    void Start()
    {
        waitTimeBeforeCatchOnFire = Random.Range(3f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
    public virtual void CatchOnFire()
    {
        if (isOnfire) return;
        StartCoroutine(CatchOnFireRoutine());
        
    }

    public virtual IEnumerator CatchOnFireRoutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
        GameManager.Instance.fireManager.SpawnFire(fireSpawnPos, 0.3f, true);
        isOnfire = true;
        fire = fireSpawnPos.GetComponentInChildren<FireMovementController>();
        fire.combustible = this;

        
    }

    public virtual void OnDestroy()
    {
        
    }



}
