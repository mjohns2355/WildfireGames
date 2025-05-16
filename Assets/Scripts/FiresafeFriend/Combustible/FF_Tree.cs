using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Tree : FF_Plants
{
    public MeshRenderer burntModel;

    private MeshRenderer burntMesh;
    protected override void Start()
    {
        base.Start();
        OnBurning += HandleBurning;

    }

    private void HandleBurning()
    {
        if (burntModel && burntMesh == null)
        {
            foreach (var mesh in meshes)
            {
                burntMesh = Instantiate(burntModel, transform);
                burntMesh.transform.position = mesh.transform.position;
                burntMesh.material = mesh.material;
                mesh.gameObject.SetActive(false);

            }
        }
    }
}
