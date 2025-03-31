using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterStructure : Structure
{
    [field:SerializeField]
    public int capacity { get; private set; }
    [field: SerializeField]
    public int availableSpace { get; private set; }
    public bool canHavePets;

    [SerializeField] GameObject relocatedCars;
    void Awake()
    {
        relocatedCars.SetActive(false);
        capacity = 20;
        availableSpace = capacity;
        //structureInfoDict.Add("Capacity", capacity);
        //structureInfoDict.Add("Available Space", availableSpace);
        //menu.assignButton.onClick.AddListener(SetAsSelectedShelter);
    }

    public override void OnStructureClick()
    {
        base.OnStructureClick(); 
        
        //if (ATC_UIController.Instance.selectedHouses.Count > 0)
        //{
        //    contextMenu.assignButton.gameObject.SetActive(true);
        //}
    }

    //void SetAsSelectedShelter()
    //{
    //    //check condition
    //    if (availableSpace == 0) return;
    //    if(!CheckIfShelterHasEnoughSpace()) return;
    //    HideUI();
    //    //ATC_UIController.Instance.selectedShelter = this;
    //    ATC_StructureModel end = GetComponentInParent<ATC_StructureModel>();
    //    foreach (var house in ATC_UIController.Instance.selectedHouses)
    //    {
    //        house.HideUI();
    //        //availableSpace -= house.pplNum;
    //        //structureInfoDict["Available Space"] = availableSpace;
    //        //menu.UpdateText(structureInfoDict);
    //        ATC_StructureModel start = house.GetComponentInParent<ATC_StructureModel>();
    //        ATC_AIDirector.Instance.SpawnACar(start, end);

    //    }
    //    ATC_UIController.Instance.selectedHouses.Clear();
    //}

    bool CheckIfShelterHasEnoughSpace()
    {
        int peopleCount = 0;
        foreach (var house in ATC_UIController.Instance.selectedHouses)
        {
            //peopleCount += house.pplNum;
        }
        return peopleCount <= availableSpace;
    }

    public void RelocateCarToShelter()
    {
        if(relocatedCars.activeSelf) return;
        relocatedCars.SetActive(true);
    }
}
