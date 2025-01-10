using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_PlantsMenu : MonoBehaviour
{
    public Transform grid;
    public GameObject plantOptionPrefab;

    FF_DirtMound currentOwner;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPlantsMenu(FF_DirtMound owner)
    {
        currentOwner = owner;

    }
    private void PopulateOptions()
    {
        foreach (var p in currentOwner.availablePlants)
        {
            var option = Instantiate(plantOptionPrefab, grid).GetComponent<FF_PlantMenuOption>();
        }
    }
}
