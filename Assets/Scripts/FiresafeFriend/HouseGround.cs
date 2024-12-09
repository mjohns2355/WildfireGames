using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGround : BaseHousePartObject
{
    [SerializeField]private float flammabilityMod;
    protected override void Start()
    {
        base.Start();

    }

    IEnumerator OnGroundPlaced()
    {
        yield return new WaitForSeconds(1);

        CheckNeighbours("Nature");

        foreach (var p in neighbours)
        {
            p.GetComponent<FF_BaseCombustible>().DecreaseFlammabilty(flammabilityMod);
        }
    }
}
