using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGround : BaseHousePartObject
{
    [SerializeField]private float flammabilityMod;
    //protected override void Start()
    //{
    //    base.Start();
    //    StartCoroutine(OnGroundPlaced());
    //}

    //IEnumerator OnGroundPlaced()
    //{
    //    yield return new WaitForSeconds(1);

    //    ApplyFlammabilityMod();
    //}

    private void ApplyFlammabilityMod()
    {
        Debug.Log("Apply Flammability Mod: " +  flammabilityMod);
        var plants = CheckPlants("Nature");

        foreach (var p in plants)
        {
            p.DecreaseFlammabilty(flammabilityMod);
        }
    }

    public List<FF_Plants> CheckPlants(string layerName)
    {
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
        List<FF_Plants> neighbours = new List<FF_Plants>();
        foreach (Collider c in colliders)
        {
            if (c != collider)
            {
                var part = c.GetComponentInParent<FF_Plants>();
                if (part != null && !part.notInteractable)
                {
                    //Debug.Log($"Added Neighbour {part.name}");
                    neighbours.Add(part);
                }

            }

        }
        return neighbours;
    }

    public override void InitHousePartObject(HouseManager owner, HousePartInfo housePart = null)
    {
        base.InitHousePartObject(owner, housePart);
        Debug.Log($"Initialize {gameObject.name}");
        flammabilityMod = ((GroundCombustibleInfo)combustibleInfo).flammabilityMod;
        ApplyFlammabilityMod();
    }
}
