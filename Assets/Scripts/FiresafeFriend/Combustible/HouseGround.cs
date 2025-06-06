using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGround : BaseHousePartObject
{
    [SerializeField]private float flammabilityMod;
    [SerializeField] private GameObject fireParticle;

    protected override void Start()
    {
        base.Start();
        OnBurnedOut += () =>
        {
            Debug.Log("Ground Burned Out");
            fireParticle.SetActive(false);
        };
    }
    private void ApplyFlammabilityMod()
    {
        //Debug.Log("Apply Flammability Mod: " +  flammabilityMod);
        var plants = CheckPlants("Nature");

        foreach (var p in plants)
        {
            p.DecreaseFlammability(flammabilityMod);
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

    private void Update()
    {
        if (!isOnFire) return;
        foreach (var mesh in meshes)
        {
            foreach (var material in mesh.materials)
            {
                material.color = Color.Lerp(mesh.material.color, burntColor, Time.deltaTime);
            }

        }
    }

    public override void InitHousePartObject(HouseManager owner, HousePartInfo housePart = null)
    {
        base.InitHousePartObject(owner, housePart);
        //Debug.Log($"Initialize {gameObject.name}");
        flammabilityMod = ((GroundCombustibleInfo)combustibleInfo).flammabilityMod;
        //flammabilityMod = ((GroundCombustibleInfo)partInfo).flammabilityMod;
        ApplyFlammabilityMod();
    }

    protected override void HandleIgnite()
    {
        base.HandleIgnite();
        Debug.Log("Ground is ignited");
        fireParticle.SetActive(true);
        
    }
}
