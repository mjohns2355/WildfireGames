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
    public override void Awake()
    {
        base.Awake();
        capacity = 20;
        availableSpace = capacity;
        structureInfoDict.Add("Capacity", capacity);
        structureInfoDict.Add("Available Space", availableSpace);
        menu.assignButton.onClick.AddListener(SetAsSelectedShelter);
    }

    public override void OnStructureClick()
    {
        base.OnStructureClick(); 
        
        if (GameManager.Instance.uiController.selectedHouses.Count > 0)
        {
            menu.assignButton.gameObject.SetActive(true);
        }
    }

    void SetAsSelectedShelter()
    {
        //check condition
        if (availableSpace == 0) return;
        if(!CheckIfShelterHasEnoughSpace()) return;
        HideUI();
        GameManager.Instance.uiController.selectedShelter = this;
        ATC_StructureModel end = GetComponentInParent<ATC_StructureModel>();
        foreach (var house in GameManager.Instance.uiController.selectedHouses)
        {
            house.HideUI();
            availableSpace -= house.pplNum;
            structureInfoDict["Available Space"] = availableSpace;
            //menu.UpdateText(structureInfoDict);
            ATC_StructureModel start = house.GetComponentInParent<ATC_StructureModel>();
            ATC_AIDirector.Instance.SpawnACar(start, end);

        }
        GameManager.Instance.uiController.selectedHouses.Clear();
    }

    bool CheckIfShelterHasEnoughSpace()
    {
        int peopleCount = 0;
        foreach (var house in GameManager.Instance.uiController.selectedHouses)
        {
            peopleCount += house.pplNum;
        }
        return peopleCount <= availableSpace;
    }
}
