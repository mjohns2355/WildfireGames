using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Props : FF_BaseCombustible
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        OnIgnite += HandleIgnite;

        HH_GameManager.Instance.inputManager.OnObjectSelected += OnPropSelected;
    }

    private void OnPropSelected(GameObject obj)
    {
        if (obj.transform.parent == transform)
        {
            //Destroy(gameObject);
            StartCoroutine(PropClickedRoutine());
        }
    }

    private void HandleIgnite()
    {
        HH_GameManager.Instance.fireManager.SpawnFire(transform.position, transform, 1f, 0.5f, true, burnTimer, 1.5f);
    }
    IEnumerator PropClickedRoutine()
    {
        gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);

    }
}
