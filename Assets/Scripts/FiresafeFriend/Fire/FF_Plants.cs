using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Plants : FF_BaseCombustible
{
    public int debris;
    public bool blocker = false;
    
    protected override void Start()
    {
        base.Start();
        OnIgnite += HandleIgnite;
        
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnPlantSelected;
    }

    private void OnPlantSelected(GameObject obj)
    {
        if(obj.transform.parent == transform)
        {
            //Debug.Log($"Clicked {gameObject.name}");
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
                    Debug.Log($"{gameObject.name} is trying to ignite {combustible.gameObject.name}");
                    combustible.TryIgnite();
                }

            }

        }
    }

    IEnumerator PlantClickedRoutine()
    {
        gameObject.SetActive(false);
        var vfx = Instantiate(Resources.Load("sticks 1"), transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        Destroy(vfx);
        Destroy(gameObject);

    }

    void OnMouseDown()
    {
        //Instantiate(Resources.Load("sticks 1"), transform.position, transform.rotation,transform);

        //Destroy(gameObject);
    }


    
    //void OnDrawGizmos()
    //{
    //    if (collider != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        //Gizmos.matrix = Matrix4x4.TRS(meshCollider.bounds.center, transform.rotation, Vector3.one);
    //        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    //    }
    //}
}
