using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class Combustible : MonoBehaviour
{
    public Transform fireSpawnPos;
    public float fireChance = 1;
    public MeshRenderer[] meshes;
    [SerializeField]bool isOnfire = false;
    [SerializeField] Color burntColor;
    FireMovementController fire;
    [SerializeField] float waitTimeBeforeCatchOnFire;
    public bool burned = false;
    private float burnTime = 0;

    private ATC_dialogManager dialog;

    // Start is called before the first frame update
    void Start()
    {
        waitTimeBeforeCatchOnFire = Random.Range(3f, 10f);
        meshes = GetComponentsInChildren<MeshRenderer>()
            .Where(meshRenderer => meshRenderer.gameObject.layer != LayerMask.NameToLayer("Ground"))
            .ToArray();
        //dialog = GameObject.FindGameObjectWithTag("Dialog").GetComponent<ATC_dialogManager>();
        dialog = GameManager.Instance.dialogManager;
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
            if(burnTime > 30 && !burned && !dialog.done)
            {

                dialog.houseDestroyed++;
                burned = true;
            }
        }
    }

    public virtual void CatchOnFire()
    {
        if (isOnfire) return;
        if (Random.Range(0.4f,1) > fireChance)
        {
            fireChance += Time.deltaTime;
            return;
        }
        if (dialog.done) return;
        StartCoroutine(CatchOnFireRoutine());
        
    }

    public virtual IEnumerator CatchOnFireRoutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeCatchOnFire);
        if (!dialog.done)
        {

            GameManager.Instance.fireManager.SpawnFire(fireSpawnPos, 0.3f, true);
            isOnfire = true;
            fire = fireSpawnPos.GetComponentInChildren<FireMovementController>();
            fire.combustible = this;
        }

        
    }




}
