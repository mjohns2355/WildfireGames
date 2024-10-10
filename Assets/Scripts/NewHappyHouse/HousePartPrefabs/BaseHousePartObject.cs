using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
public class BaseHousePartObject : MonoBehaviour
{
    public HousePart housePart;
    public MeshRenderer meshRenderer;
    public HouseNode houseNode;
    public HouseManager houseManager;
    public HousePartType HousePartType { get; private set; }
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
        houseNode = new HouseNode(housePart);
        HousePartType = housePart.housePartType;
        gameObject.layer = LayerMask.NameToLayer("Structure");
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
        HouseNode newNode = new HouseNode(newPart.housePart);
        newPart.houseNode = newNode;
        houseManager.houseGraph.AddHousePart(newPart.housePart);

        // Reconnect the new node to the neighbors of the old node
        foreach (var neighbor in neighbors)
        {
            houseManager.houseGraph.ConnectParts(newNode, neighbor);
        }

        Destroy(gameObject);
    }
    

}
