using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using HappyHouse.FireSystem;
using UnityEngine.EventSystems;
using System;


public class BaseHousePartObject : MonoBehaviour
{
    //public HousePart housePart;
    public MeshRenderer meshRenderer;
    public HouseNode houseNode;
   /* public*/ HouseManager owner;
    public HousePartType HousePartType { get; private set; }
    public float durability;
    public float flammability;
    public bool isOnFire = false;
    public float burnDuration = 100f;
    public BurnStage burnStage = BurnStage.Igniting;
    //[SerializeField] Material material;
    public bool isOnCursor = false;
    public HousePartInfo PartInfo { get; private set; }
    private float burnTimer = 0f;
    private Rigidbody rb;
    private void Start()
    {
        //InitHousePartObject();
        rb = GetComponent<Rigidbody>();
        HH_GameManager.Instance.UIManager.inventoryUI.onCategoryItemButtonClicked += ReplaceHousePartObject;
    }



    private void Update()
    {
        if (!isOnCursor) return;
        

    }
    public void InitHousePartObject(HousePartInfo housePart, HouseManager owner)
    {
        //houseNode = new HouseNode(housePart);
        //houseNode = new HouseNode(this);
        var mesh = Instantiate(housePart.mesh, transform);
        meshRenderer = mesh.GetComponent<MeshRenderer>();
        HousePartType = housePart.housePartType;
        //gameObject.layer = LayerMask.NameToLayer("Structure");
        durability = housePart.durability;
        flammability = housePart.flammability;
        PartInfo = housePart;
        this.owner = owner;
        ReplaceMeshMaterial(housePart.material);
    }

    void ReplaceMeshMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public void ReplaceHousePartObject(BaseHousePartObject newPart)
    {
        // TO DO: Improve this later
        // Only current player and selected house part should invoke this function
        if(newPart.PartInfo.housePartType != HousePartType) return;
        if (HH_GameManager.Instance.currentPlayer != owner) return;
        

        List<HouseNode> neighbors = new List<HouseNode>(houseNode.neighbourNodes);

        owner.houseGraph.RemoveHousePart(houseNode);
        newPart.transform.parent = transform.parent;
        newPart.gameObject.transform.position = transform.position;
        newPart.gameObject.transform.rotation = transform.rotation;
        newPart.gameObject.transform.localScale = transform.localScale;

        newPart.owner = owner;    
        // Initialize the new part's HouseNode and add it to the HouseGraph
        HouseNode newNode = new HouseNode(newPart);
        newPart.houseNode = newNode;
        owner.houseGraph.AddHousePart(newPart);

        // Reconnect the new node to the neighbors of the old node
        foreach (var neighbor in neighbors)
        {
            owner.houseGraph.ConnectParts(newNode, neighbor);
        }
        Debug.Log($"Replace {gameObject.name} with {newPart.name}");
        HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(HousePartType);
        Destroy(gameObject);
    }


    private IEnumerator IgniteWithDelay()
    {
        // Calculate delay based on flammability (inverse relationship)
        float ignitionDelay = Mathf.Clamp(5f / flammability, 0.5f, 5f);
        yield return new WaitForSeconds(ignitionDelay);

        if (!isOnFire)
        {
            isOnFire = true;
            burnTimer = burnDuration / flammability; // Higher flammability = shorter burn duration
            HH_GameManager.Instance.fireManager.SpawnFire(transform, 1, true);
            StartCoroutine(Burn());
        }
    }

    public void Ignite()
    {
        //Debug.Log($"{gameObject.name} is ignited");
        
        //if(!isOnFire && flammability > 0)
        //{
        //    isOnFire = true;
        //    burnTimer = burnDuration;
        //    HH_GameManager.Instance.fireManager.SpawnFire(transform, 1, true);
        //    StartCoroutine(Burn());
        //}
        StartCoroutine(IgniteWithDelay());
    }

    IEnumerator Burn()
    {
        while(burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;
            durability -= flammability * Time.deltaTime;

            if( durability <= 0 )
            {
                SpreadFireToNeighbour();
                DestroyHousePart();
                yield break;
            }

            yield return null;
        }

        SpreadFireToNeighbour();
        isOnFire = false;
    }

    private void SpreadFireToNeighbour()
    {
        if(owner == null || houseNode == null)
        {
            Debug.Log("No valid house node");
            return;
        }

        Debug.Log("Spread to neighbour");
        var houseGraph = owner.houseGraph;
        var neighbours = houseGraph.GetNeighbors(houseNode);

        foreach ( var neighbor in neighbours)
        {
            var housePartObj = neighbor.housePart;
            if (housePartObj != null && !housePartObj.isOnFire)
            {
                housePartObj.Ignite();
            }
        }
    }

    private void DestroyHousePart()
    {
        Destroy(gameObject);
    }
}
