using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHousePartObject : MonoBehaviour
{
    public HousePart housePart;
    public MeshRenderer meshRenderer;
    public HouseNode houseNode;
    //[SerializeField] Material material;

    private void Start()
    {
        InitHousePartObject();
    }

    void InitHousePartObject()
    {
        houseNode = new HouseNode(housePart);
        ReplaceMeshMaterial(housePart.material);
    }

    void ReplaceMeshMaterial(Material material)
    {
        meshRenderer.material = material;
    }
}
