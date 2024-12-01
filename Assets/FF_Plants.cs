using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Plants : MonoBehaviour
{
    public int debris;
    public bool blocker = false;
    FF_Combustible combustible;
    [SerializeField]Collider collider;
    private void Start()
    {
        collider = GetComponent<Collider>();
        if (TryGetComponent(out combustible))
        {
            combustible.OnIgnite += HandleIgnite;
        }
    }

    private void HandleIgnite()
    {
        Vector3 center = collider.bounds.center;
        Vector3 halfExtents = collider.bounds.extents;
        LayerMask layerMask = LayerMask.GetMask("Structure");
        //Debug.Log($"{gameObject.name}'s extent size: {halfExtents.magnitude}");
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
        
        foreach (Collider c in colliders)
        {
            if (c != collider)
            {
                var part = c.GetComponentInParent<FF_Combustible>();
                if (part != null)
                {
                    //Debug.Log($"{gameObject.name} is trying to ignite {part.gameObject.name}");
                    part.TryIgnite();
                }

            }

        }
    }

    void OnMouseDown()
    {
        Instantiate(Resources.Load("sticks 1"), transform.position, transform.rotation);
        //if (GameObject.FindGameObjectWithTag("Dialog") == null)
        //{
        //    GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().Clear(debris);
        //} 
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.cyan;
            //Gizmos.matrix = Matrix4x4.TRS(meshCollider.bounds.center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
