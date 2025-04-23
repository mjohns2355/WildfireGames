using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
public class Combustible : MonoBehaviour
{
    public Transform fireSpawnPos;
    public float fireChance = 1;
    public List<MeshRenderer> meshes;
    [SerializeField]protected bool isOnfire = false;
    [SerializeField]protected Color burntColor;
    FireMovementController fire;
    [SerializeField]protected float waitTimeBeforeCatchOnFire;
    [SerializeField]protected float particleScale = 0.3f;
    public UnityEvent OnIgnite;
    public UnityEvent OnStopIgniting;
    public bool burned = false;
    public bool canBurn = true;
    protected float burnTime = 0;

    //private ATC_dialogManager dialog;

    // Start is called before the first frame update
    public virtual void Start()
    {
        waitTimeBeforeCatchOnFire = Random.Range(3f, 10f);
        if(meshes.Count == 0)
        {
            meshes = GetComponentsInChildren<MeshRenderer>()
            .Where(meshRenderer => meshRenderer.gameObject.layer != LayerMask.NameToLayer("Ground"))
            .ToList();
        }
        //dialog = GameObject.FindGameObjectWithTag("Dialog").GetComponent<ATC_dialogManager>();
       
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!canBurn) return;
        if (isOnfire && !burned)
        {
            burnTime += Time.deltaTime;
            foreach(MeshRenderer m in meshes)
            {
                m.material.color = Color.Lerp(m.material.color, burntColor, Time.deltaTime);
            }
            if(burnTime > 30 && !burned && !GameManager.Instance.SimIsEnd)
            {
                if(gameObject.layer == LayerMask.NameToLayer("Structure"))
                {
                    GameManager.Instance.housesDestroyed++;
                }
                //dialog.houseDestroyed++;
                burned = true;
            }
        }
    }

    public virtual void CatchOnFire()
    {
        if (!canBurn) return;
        if (isOnfire || burned) return;
        //if (fire != null && fire.isInFireSafeZone) return;

        if (Random.Range(0.4f,1) > fireChance)
        {
            fireChance += Time.deltaTime;
            return;
        }
        if (GameManager.Instance.SimIsEnd) return;
        StartCoroutine(CatchOnFireRoutine());
        
    }

    public virtual void StopFire()
    {
        StopAllCoroutines();
        OnStopIgniting.Invoke();
        isOnfire = false;

        var fires = GetComponentsInChildren<FireMovementController>();

        foreach(var f in fires)
        {
            Destroy(f.gameObject);
        }

        if (fire != null)
        {
            fire = null;
        }

    }
    
    public virtual IEnumerator CatchOnFireRoutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
        if (!GameManager.Instance.SimIsEnd)
        {

            GameManager.Instance.fireManager.SpawnFire(fireSpawnPos, particleScale, true);
            
            isOnfire = true;
            fire = fireSpawnPos.GetComponentInChildren<FireMovementController>();
            OnIgnite.Invoke();
            if (fire != null)
            {
                fire.combustible = this;
            }

        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        if (hit == null) return;
        // Check for fire-safe zone first
        if (hit.layer == LayerMask.NameToLayer("FireSafe"))
        {
            StopFire();
            canBurn = false;

            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hit = other.gameObject;
        if (hit != null && hit.layer == LayerMask.NameToLayer("FireSafe"))
        {
            canBurn = true;
        }
    }


}
