using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using HappyHouse.FireSystem;
using UnityEngine.EventSystems;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;


public class BaseHousePartObject : FF_BaseCombustible
{
    public MeshRenderer[] meshes;
    public HouseNode houseNode;
    HouseManager owner;
    public HousePartType HousePartType { get; private set; }

    public bool isOnCursor = false;
    public HousePartInfo partInfo;

    public List<BaseHousePartObject> neighbours = new List<BaseHousePartObject>();
    //public FF_BaseCombustible combustible;
    protected override void Awake()
    {
        base.Awake();
        if (notInteractable) return;
        meshes = GetComponentsInChildren<MeshRenderer>();
        if (combustibleInfo != null)
        {
            partInfo = (HousePartInfo)combustibleInfo;
        }
    }
    protected override void Start()
    {
        //InitHousePartObject();
        base.Start();

        OnIgnite += HandleIgnite;
        OnBurnedOut += HandleBurnedOut;
        //if(TryGetComponent(out combustible))
        //{
        //    combustible.OnIgnite += HandleIgnite;
        //    combustible.OnBurnedOut += HandleBurnedOut;
        //}
        //HH_GameManager.Instance.UIManager.inventoryUI.onCategoryItemButtonClicked += ReplaceHousePartObject;
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnHousePartSelected;
    }

    private void OnHousePartSelected(GameObject obj)
    {
        //TryGetComponent(out BaseHousePartObject part);
        if(obj.transform.parent == transform && !notInteractable)
        {
            
            HH_GameManager.Instance.uiManager.ShowStoreScreen(partInfo, bubble);
        }
        
    }

    public virtual void InitHousePartObject(HouseManager owner, HousePartInfo housePart = null)
    {
        

        var part = housePart == null ? partInfo : housePart;
        //Debug.Log($"Initialize {gameObject.name}");
        HousePartType = part.housePartType;
        durability = part.durability;
        flammability = part.flammability;
        //}
        //if (combustible != null)
        //{
        //    combustible.
        //    combustible.
        partInfo = part;
        this.owner = owner;
        //houseNode = new HouseNode(this);
        ReplaceMeshMaterial(part.material);
        //CheckNeighbours();
        //Debug.Log($"Part: {houseNode.housePart.name}");
    }

    void ReplaceMeshMaterial(Material material)
    {
        //Debug.Log($"Replace material with {material.name}");
        foreach (var mesh in meshes)
        {
            Debug.Log(mesh.name);
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
        //Debug.Log($"{gameObject.name}'s extent size: {halfExtents.magnitude}");
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
                    //Debug.Log($"Added Neighbour {part.name}");
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
        if (owner == null || houseNode == null)
        {
            Debug.Log("No valid house node");
            yield break;
        }

        
        while (isOnFire)
        {
            var houseGraph = owner.houseGraph;
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

    //private void DestroyHousePart()
    //{
    //    //Debug.Log($"{gameObject.name} is destroyed");
    //    isOnFire = false;
    //    StopAllCoroutines();
    //    Destroy(gameObject);
    //}

    private void HandleIgnite()
    {
        SpawnFire();
        StartCoroutine(SpreadFireToNeighbour());
        UpdateMaterial(burnStage);
    }

    private void HandleBurnedOut()
    {

    }

    private void SpawnFire()
    {
        var top = collider.bounds.max;
        var bottom = collider.bounds.min;
        var center = collider.bounds.center;
        var pos = new Vector3(center.x, bottom.y, center.z);
        var end = new Vector3(center.x, top.y, center.z);
        var fire = HH_GameManager.Instance.fireManager.SpawnFire(pos, transform,2f, 0.1f, true, burnTimer);
        fire.canMove = true;
        fire.startPos = pos;
        fire.endPos = end;
    }
}
