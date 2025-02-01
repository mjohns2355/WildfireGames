using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_PlantsMenu : MonoBehaviour
{
    public Transform grid;
    public GameObject plantOptionPrefab;

    FF_DirtMound currentOwner;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        transform.position = screenPos;
    }

    public void ShowPlantsMenu(FF_DirtMound owner)
    {
        ClearOptions();
        currentOwner = owner;
        pos = owner.menuPos;
        currentOwner.OnPlanted += OnPlanted;
        PopulateOptions();
    }

    public void ClosePlantsMenu()
    {
        if(currentOwner != null)
        {
            currentOwner.OnPlanted -= OnPlanted;
            currentOwner = null;
        }

        ClearOptions();
        gameObject.SetActive(false);
        
    }
    private void PopulateOptions()
    {
        foreach (var p in currentOwner.availablePlants)
        {
            var option = Instantiate(plantOptionPrefab, grid).GetComponent<FF_PlantMenuOption>();
            option.InitPlantMenuOption(p,currentOwner);
        }
        var removeButton = Instantiate(plantOptionPrefab, grid).GetComponent<FF_PlantMenuOption>();
        removeButton.InitRemoveButton(currentOwner);
    }

    private void ClearOptions()
    {
        for(int i = 0; i< grid.childCount; i++)
        {
            Destroy(grid.GetChild(i).gameObject);
        }
    }

    private void OnPlanted()
    {
        
        ClosePlantsMenu();
        //ClearOptions();
        //PopulateOptions() ;
    }
    
}
