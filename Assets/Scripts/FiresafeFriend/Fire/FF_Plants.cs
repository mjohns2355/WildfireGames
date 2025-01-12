using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Plants : FF_BaseCombustible
{
    public int debris;
    public bool isClickable = true;
    
    protected override void Start()
    {
        base.Start();
        OnIgnite += HandleIgnite;
        OnCombustibleDestroyed += HandleBurnedOut;
        //HH_GameManager.Instance.inputManager.OnObjectSelected += OnPlantSelected;
    }

    private void HandleBurnedOut()
    {
        Destroy(gameObject);
    }

    protected override void OnCombustibleClicked(GameObject obj)
    {
        //if (obj.transform.parent == transform)
        if (obj == gameObject && isClickable)
        {
            Debug.Log($"Clicked {gameObject.name}");
            StartCoroutine(PlantClickedRoutine());
        }
    }
    private void HandleIgnite()
    {
        HH_GameManager.Instance.fireManager.SpawnFire(transform.position, transform,1f, 0.5f, true, burnTimer, 1.5f);
        Vector3 center = collider.bounds.center;
        Vector3 halfExtents = collider.bounds.extents;
        LayerMask layerMask = LayerMask.GetMask("Structure");
        //Debug.Log($"{gameObject.name}'s extent size: {halfExtents.magnitude}");
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
        
        foreach (Collider c in colliders)
        {
            if (c != collider)
            {
                FF_BaseCombustible combustible;
                if(c.gameObject.layer == LayerMask.NameToLayer("Structure"))
                {
                    combustible = c.GetComponentInParent<FF_BaseCombustible>();
                }
                else
                {
                    combustible = c.GetComponent<FF_BaseCombustible>();
                }
  
                if (combustible != null)
                {
                    //Debug.Log($"{gameObject.name} is trying to ignite {combustible.gameObject.name}");
                    combustible.TryIgnite();
                }

            }

        }
    }

    IEnumerator PlantClickedRoutine()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        var vfx = Instantiate(Resources.Load("sticks 1"), transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        Destroy(vfx);
        Destroy(gameObject);

    }

    
}
