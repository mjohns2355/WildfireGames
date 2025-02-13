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
        foreach (FC_Tree tree in trees)
        {
            var mesh = tree.normal.GetComponentsInChildren<MeshRenderer>();
            foreach(var m in mesh)
            {
                if (m.CompareTag("Tree"))
                {
                    meshes.Add(m);
                }
            }
        }
    }
    public override void Update()
    {
        if (isOnfire && !burned)
        {
            burnTime += Time.deltaTime;
            foreach (MeshRenderer m in meshes)
            {
                m.material.color = Color.Lerp(m.material.color, burntColor, Time.deltaTime);
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
