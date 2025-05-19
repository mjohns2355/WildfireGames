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

    public override void OnCombustibleClicked(GameObject obj)
    {
        if (obj == gameObject && isClickable)
        {
            Debug.Log($"Clicked {gameObject.name}");
            HH_GameManager.Instance.uiManager.purchasePopup.confirmRemove.onClick.AddListener(RemoveTree);
            HH_GameManager.Instance.uiManager.ShowPurchasePopup(null, false,true);
        }
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

    private void RemoveTree()
    {
        if (HH_GameManager.Instance.currentPlayer.budgetManager.SpendBudget(5000))
        {
            StartCoroutine(PlantClickedRoutine());
            HH_GameManager.Instance.uiManager.HidePurchasePopup(null);
        }
        else
        {
            HH_GameManager.Instance.uiManager.ShowPurchasePopup(null, true);
        }
        
    }
}
