using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_PlantsMenu : MonoBehaviour
{
    public Transform grid;
    public GameObject plantOptionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopulateOptions()
    {
        foreach (var p in ResourceManager.Instance.plants)
        {
            
        }
    }
}
