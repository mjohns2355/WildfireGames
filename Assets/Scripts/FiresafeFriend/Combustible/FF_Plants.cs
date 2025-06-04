using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Plants : FF_BaseCombustible
{
    public int debris;

    public bool canClickToRemove = false;
    public Action onPlantClicked;
    protected override void Start()
    {
        base.Start();
        OnIgnite += HandleIgnite;
        OnCombustibleDestroyed += _=> HandleBurnedOut();
        HH_GameManager.Instance.OnPlantModeChanged += (isPlantMode) =>
        {
            isClickable = isPlantMode;
        };
        //HH_GameManager.Instance.inputManager.OnObjectSelected += OnPlantSelected;
    }


    private void HandleBurnedOut()
    {
        gameObject.SetActive(false);

    }

    protected override void OnDestroy()
    {
        //Debug.Log($"On {gameObject.name} destroyed");
        base.OnDestroy();
        onPlantClicked = null;
        OnCombustibleDestroyed = null;
        OnIgnite = null;
    }

    public override void OnCombustibleClicked(GameObject obj)
    {
        if (this == null || gameObject == null) return;
        if (obj == gameObject && isClickable)
        {
            //Debug.Log($"Clicked {gameObject.name}");
            StartCoroutine(PlantClickedRoutine());
        }
    }
    protected virtual void HandleIgnite()
    {
        HH_GameManager.Instance.fireManager.SpawnFire(transform.position, transform,1f, 3f, true, burnTimer, 4f);
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

    protected IEnumerator PlantClickedRoutine()
    {
        if (this == null || gameObject == null) yield break;
        if (canClickToRemove)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            var vfx = Instantiate(Resources.Load("sticks 1"), transform.position, transform.rotation);
            collider.enabled = false;
            yield return new WaitForSeconds(1f);
            Destroy(vfx);
            //Destroy(gameObject);
            transform.GetChild(0).gameObject.SetActive(true);
            gameObject.SetActive(false);
            OnCombustibleDestroyed?.Invoke(this);
        }
        else
        {
            //Debug.Log("Invoke onPlantClicked");
            onPlantClicked?.Invoke();
        }

    }



    
}
