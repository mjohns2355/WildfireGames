using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_TreeGroup : Combustible
{
    public FC_Tree[] trees;

    public override void Start()
    {
        
        waitTimeBeforeCatchOnFire = Random.Range(3f, 10f);
        trees = GetComponentsInChildren<FC_Tree>();
    }
    public override void Update()
    {
        //Debug.Log($"isOnfire: {isOnfire}, burned: {burned}");
        if (isOnfire && !burned)
        {
            burnTime += Time.deltaTime;
            foreach (FC_Tree tree in trees)
            {

                var m = tree.normal.GetComponentInChildren<MeshRenderer>();
                var c = m.material.GetColor("_Color");
                var color = Color.Lerp(c, burntColor, Time.deltaTime);
                m.material.SetColor("_Color", color);
            }
            if (burnTime > 30 && !burned && !GameManager.Instance.SimIsEnd)
            {
                foreach (FC_Tree tree in trees)
                {
                    tree.IsBurnt = true;
                }
                burned = true;
            }
        }
    }

}
