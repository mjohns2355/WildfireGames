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
    public MeshRenderer[] meshes;
    [SerializeField]bool isOnfire = false;
    [SerializeField] Color burntColor;
    FireMovementController fire;
    [SerializeField] float waitTimeBeforeCatchOnFire;
    public UnityEvent OnIgnite;
    public bool burned = false;
    private float burnTime = 0;
    
    //private ATC_dialogManager dialog;

    // Start is called before the first frame update
    void Start()
    {
        waitTimeBeforeCatchOnFire = Random.Range(3f, 10f);
        meshes = GetComponentsInChildren<MeshRenderer>()
            .Where(meshRenderer => meshRenderer.gameObject.layer != LayerMask.NameToLayer("Ground"))
            .ToArray();
        //dialog = GameObject.FindGameObjectWithTag("Dialog").GetComponent<ATC_dialogManager>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnfire && !burned)
        {
            burnTime += Time.deltaTime;
            foreach(MeshRenderer m in meshes)
            {
                m.material.color = Color.Lerp(m.material.color, burntColor, Time.deltaTime);
            }
            if(burnTime > 30 && !burned && !GameManager.Instance.SimIsEnd)
            {
                GameManager.Instance.housesDestroyed++;
                //dialog.houseDestroyed++;
                burned = true;
            }
        }
    }

    public virtual void CatchOnFire()
    {
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

    
    public virtual IEnumerator CatchOnFireRoutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
        if (!GameManager.Instance.SimIsEnd)
        {

            GameManager.Instance.fireManager.SpawnFire(fireSpawnPos, 0.3f, true);
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
            this.enabled = false;
            return;
        }
    }


}
