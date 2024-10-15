using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using HappyHouse.FireSystem;
using UnityEngine.EventSystems;
using System;
public class BaseHousePartObject : MonoBehaviour, IPointerClickHandler
{
    public HousePart housePart;
    public MeshRenderer meshRenderer;
    public HouseNode houseNode;
    public HouseManager houseManager;
    public HousePartType HousePartType { get; private set; }

    public float durability;
    public float flammability;
    public bool isOnFire = false;
    public float burnDuration = 5f;
    private float burnTimer = 0f;

    //[SerializeField] Material material;
    public bool isOnCursor = false;
    private void Start()
    {
        InitHousePartObject();

    }

    private void Update()
    {
        if (!isOnCursor) return;
        

    }
    void InitHousePartObject()
    {
        //houseNode = new HouseNode(housePart);
        //houseNode = new HouseNode(this);
        HousePartType = housePart.housePartType;
        //gameObject.layer = LayerMask.NameToLayer("Structure");
        durability = housePart.durability;
        flammability = housePart.flammability;
        ReplaceMeshMaterial(housePart.material);
    }

    void ReplaceMeshMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public void ReplaceHousePartObject(BaseHousePartObject newPart)
    {
        List<HouseNode> neighbors = new List<HouseNode>(houseNode.neighbourNodes);

        houseManager.houseGraph.RemoveHousePart(houseNode);

        newPart.gameObject.transform.position = transform.position;
        newPart.gameObject.transform.rotation = transform.rotation;
        newPart.gameObject.transform.localScale = transform.localScale;

        newPart.houseManager = houseManager;    
        // Initialize the new part's HouseNode and add it to the HouseGraph
        HouseNode newNode = new HouseNode(newPart);
        newPart.houseNode = newNode;
        houseManager.houseGraph.AddHousePart(newPart);

        // Reconnect the new node to the neighbors of the old node
        foreach (var neighbor in neighbors)
        {
            houseManager.houseGraph.ConnectParts(newNode, neighbor);
        }

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} is on click.");
    }


    public void Ignite()
    {
        Debug.Log($"{gameObject.name} is ignited");
        
        if(!isOnFire && flammability > 0)
        {
            isOnFire = true;
            burnTimer = burnDuration;
            HH_GameManager.Instance.fireManager.SpawnFire(transform, 1, true);
            StartCoroutine(Burn());
        }
    }

    IEnumerator Burn()
    {
        while(burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;
            durability -= flammability * Time.deltaTime;

            if( durability < 0 )
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
        if(houseManager == null || houseNode == null)
        {
            Debug.Log("No valid house node");
            return;
        }

        Debug.Log("Spread to neighbour");
        var houseGraph = houseManager.houseGraph;
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
