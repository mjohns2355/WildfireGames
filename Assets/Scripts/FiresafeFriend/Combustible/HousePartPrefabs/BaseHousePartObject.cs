using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using HappyHouse.FireSystem;
using UnityEngine.EventSystems;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Linq;
using DG.Tweening;


public class BaseHousePartObject : FF_BaseCombustible
{

    public MeshRenderer burntModel;
    public HouseNode houseNode;
    public Transform bubblePos;
    public GameObject VFX;
    public Transform vfxPos;
    public HouseManager Owner { get; private set; }
    public HousePartType HousePartType { get; private set; }

    //public bool isOnCursor = false;
    public HousePartInfo partInfo, defaultPartInfo;
    public Material burnMaterial;
    public List<BaseHousePartObject> neighbours = new List<BaseHousePartObject>();
    private MeshRenderer burntMesh;
    //public bool isTesting;
    protected override void Start()
    {
        base.Start();
        isClickable = true;
        OnIgnite += HandleIgnite;
        OnCombustibleDestroyed += HandleDestroy;
        OnBurning += HandleBurning;
        OnBurnedOut += HandleBurnedOut;
        if (notInteractable) return;


        foreach (var mesh in meshes)
        {
            mesh.gameObject.layer = LayerMask.NameToLayer("Structure");
        }
        if (combustibleInfo != null)
        {
            partInfo = (HousePartInfo)combustibleInfo;
        }

        //if (isTesting)
        //{
        //    InitHousePartObject(HH_GameManager.Instance.currentPlayer);
        //}
    }

    private void Update()
    {
        if (burntMesh && isOnFire)
        {
            burntMesh.material.color = Color.Lerp(burntMesh.material.color, burntColor, Time.deltaTime);
        }
        
    }
    public override void OnCombustibleClicked(GameObject obj)
    {
        
        if (obj.transform.parent == transform && !notInteractable && isClickable)
        {

            HH_GameManager.Instance.uiManager.ShowStoreScreen(partInfo.housePartType,partInfo.isPublic);
            
        }

        
    }

    public virtual void InitHousePartObject(HouseManager owner, HousePartInfo housePart = null)
    {
        var part = housePart == null ? partInfo : housePart;
        HousePartType = part.housePartType;
        durability = part.durability;
        flammability = part.flammability;
        partInfo = part;
        combustibleInfo = partInfo;
        Owner = owner;
        ReplaceMeshMaterial(part.material);
        switch (HousePartType)
        {
            case HousePartType.Wall:
                VFX = ResourceManager.Instance.VFXs[HousePartType.Wall];
                break;
            case HousePartType.Window:
                VFX = ResourceManager.Instance.VFXs[HousePartType.Window];
                break;
        }

        defaultPartInfo = part;
    }

    
    void ReplaceMeshMaterial(Material material)
    {
        //Debug.Log($"Replace material with {material.name}");
        foreach (var mesh in meshes)
        {
            mesh.material = material;
        }
        
    }

    public virtual List<BaseHousePartObject> CheckNeighbours(string layerName)
    {
        //Debug.Log($"Checking {gameObject.name}'s neighbours");
        Vector3 center = collider.bounds.center;
        Vector3 halfExtents = collider.bounds.extents;
        LayerMask layerMask = LayerMask.GetMask(layerName);
        if (halfExtents.magnitude < 2)
        {
            halfExtents = new Vector3(2, 2, 2);
        }
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
        //Debug.Log($"Found {colliders.Length} colliders overlapping with the bounds of {gameObject.name}.");
        List<BaseHousePartObject> neighbours = new List<BaseHousePartObject>();
        foreach (Collider c in colliders)
        {
            if (c != collider)
            {
                var part = c.GetComponentInParent<BaseHousePartObject>();
                if (part != null && !part.notInteractable)
                {
                    neighbours.Add(part);
                }

            }

        }
        this.neighbours = neighbours;
        return neighbours;
    }

   
    private void UpdateMaterial(BurnStage burnStage)
    {
        //throw new NotImplementedException();
    }

    private IEnumerator SpreadFireToNeighbour()
    {
        if(gameObject.tag == "Fence")
        {
            yield break;
        }
        if (Owner == null || houseNode == null)
        {
            Debug.Log("No valid house node");
            yield break;
        }
        while (isOnFire)
        {
            var houseGraph = Owner.houseGraph;
            var neighbours = houseGraph.GetNeighbors(houseNode);
            //Debug.Log("Spread to neighbour");
            foreach (var neighbor in neighbours)
            {
                var housePartObj = neighbor.housePart;
                
                if (housePartObj != null && !housePartObj.isOnFire)
                {
                    
                    // Calculate the delay based on distance
                    float distance = Vector3.Distance(transform.position, housePartObj.transform.position);
                    float spreadDelay = Mathf.Clamp(distance * 0.5f, 1f, 5f);
                    
                    // Wait for the calculated spread delay before attempting to ignite
                    yield return new WaitForSeconds(spreadDelay);
                    if(housePartObj != null)
                    {
                        housePartObj.TryIgnite();
                    }
                   
                }
            }


            yield return new WaitForSeconds(10f);
        }
    }

    private void HandleBurning()
    {
        if (burntModel && burntMesh == null)
        {
            foreach (var mesh in meshes)
            {
                burntMesh = Instantiate(burntModel, transform);
                burntMesh.transform.position = mesh.transform.position;
                burntMesh.material = mesh.material;
                mesh.gameObject.SetActive(false);

            }
        }

        
    }
    private void HandleIgnite()
    {
        SpawnFire();        
        StartCoroutine(SpreadFireToNeighbour());
    }

    private void HandleBurnedOut()
    {
        //Debug.Log("Burnt");

    }

    private void HandleDestroy()
    {
        StopAllCoroutines();
        if(VFX != null)
        {
            //StartCoroutine(DestroyRoutine());
            foreach (var m in meshes)
            {
                m.gameObject.SetActive(false);
            }
            var vfx = Instantiate(VFX, vfxPos.position, Quaternion.identity, vfxPos);
            //yield return new WaitForSeconds(1f);
            DOVirtual.DelayedCall(1f, () =>
            {
                Destroy(vfx);
                Destroy(gameObject);
            });
           
        }
        else
        {
            Destroy(gameObject);
        }
        Owner.burnedWeight += HousePartType.GetHousePartWeight();
    }


    private void SpawnFire()
    {
        //Debug.Log($"Burn Timer: {burnTimer}");
        
        var fire = HH_GameManager.Instance.fireManager.SpawnFire(bottomPosition, transform,2f, 0.2f, true, burnTimer);
        fire.canMove = true;
        fire.startPos = bottomPosition;
        fire.endPos = topPosition;
    }

}
