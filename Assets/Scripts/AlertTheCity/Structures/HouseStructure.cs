//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//[ExecuteInEditMode]



public class HouseStructure : Structure
{
    public bool isMainHouse;
    //public HouseInfo info;
    public HouseTypeInfo houseInfo;
    public HouseType houseType;
    [SerializeField] List<HouseStructure> sameTypeHouses = new List<HouseStructure>();
    [SerializeField] GameObject[] houseModels;
    [SerializeField] Transform mesh;
    [SerializeField] List<HouseChoice> choices = new List<HouseChoice>();
    [SerializeField] Material metalRoofMaterial;
    public int petNum = 0;
    public int carNum = 1;
    public int horseNum = 0;
    public int kidNum = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public float carSpawnWaitTime = 0f;
    //public bool hasHorseTrailer = false;
    public bool hasHorseTrailer
    {
        get { return houseType == HouseType.horse && horseNum != 0; }
        set { hasHorseTrailer = value; }
    }

    public bool hasKidsToPickup
    {
        get { return houseType == HouseType.kids && kidNum != 0; }
        set { hasKidsToPickup = value; }
    }

    string lastOption = string.Empty;
    string currentOption = "Wait for Notice"; //default option
    ATC_PlacementManager placementManager;
    Combustible combustible;
    MeshRenderer currentHouseModel;
    ATC_StructureModel targetShelter;
    private void Start()
    {
        combustible =  GetComponent<Combustible>();
        placementManager = GameManager.Instance.structureManager.placementManager;
        
        if (isMainHouse)
        {
            // only main house has info
            //info = new HouseInfo(houseType,this);
            menu.icon.SetActive(true);
            List<ATC_StructureModel> houses = placementManager.GetAllHouses();
            GameManager.Instance.uiController.AddMenu(menu);
            foreach (var house in houses)
            {
                if(house == null) continue;
                var houseStructure = house.GetComponent<HouseStructure>();
                if (houseStructure == null) continue;
                if (houseStructure.houseType == houseType)
                {
                    sameTypeHouses.Add(houseStructure);
                    foreach(var h in sameTypeHouses)
                    {
                        //h.choices = info.choices;

                        h.houseInfo = houseInfo;
                        h.houseInfo.InitHouseInfo(h);
                    }
                }
            }
            menu.onOptionSelected += OnOptionButtonClicked;
            //GameManager.Instance.structureManager.allMainHouses.Add(this);
            targetShelter = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.Shelter);
        }

    }

    private void OnEnable()
    {
        InitHouseModel();
    }

    void InitHouseModel()
    {
        if (mesh.childCount >= 1) return;
        GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        currentHouseModel = Instantiate(houseModel, transform.position, mesh.transform.rotation, mesh).GetComponentInChildren<MeshRenderer>();
    }
    public void RandomizeHouseType()
    {
        // 0 is None
        houseType = (HouseType)Random.Range(1, System.Enum.GetValues(typeof(HouseType)).Length);
    }

    public void SetHouseType(HouseType type)
    {
        houseType = type;
        
    }
    public override void OnStructureClick()
    {
        
        foreach (var house in sameTypeHouses)
        {
            house.outline.enabled = true;
            GameManager.Instance.uiController.AddSelectedHouse(house);
        }
    }

    public override void StopSturctureClick()
    {
        foreach (var house in sameTypeHouses)
        {
            house.outline.enabled=false;
        }
    }
    public void AfterSpawnACar()
    {
        if (carNum <= 0) return;

        carNum--;

    }

    public bool CanSpawnCar()
    {

        return carNum > 0;
        
    }



    void OnOptionButtonClicked(OptionButton button)
    {
        var option = button.GetOptionContent();
        if (option == null) return;
        currentOption = option;

        
    }

    void ApplyChoice()
    {
        choices = houseInfo.normalChoices.Union(houseInfo.lockedChoices).ToList();

        foreach(var choice in choices)
        {
            if(choice.choiceName == currentOption)
            {
                //Debug.Log("current option: " + currentOption);
                if(lastOption == currentOption) return;

                foreach (var house in sameTypeHouses)
                {
                    choice.ApplyEffect(house);
                }
                //choice.ApplyEffect(this);
                lastOption = currentOption;
                break;
            }
            else
            {
                // default spawn time for house type doesn't have 'Wait for Notice' option
                carSpawnWaitTime = 5;
            }
        }
        

    }
    public IEnumerator SpawnCarRoutine()
    {
        ApplyChoice();
        yield return new WaitForSeconds(carSpawnWaitTime);
        Debug.Log("After "+ carSpawnWaitTime + "sec(s), "+ houseType + " Spawned " + carNum +" " + carSpeed + " speed car(s)");
        //destination shelter

        foreach (var house in sameTypeHouses)
        {

            if (hasKidsToPickup)
            {
                var school = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.School);

                ATC_AIDirector.Instance.SpawnCarWithMultipleStops(house.GetComponent<ATC_StructureModel>(), new List<ATC_StructureModel> { school, targetShelter }, carSpeed, carNum);
            }
            else
            {
                ATC_AIDirector.Instance.SpawnACar(house.GetComponent<ATC_StructureModel>(), targetShelter, carSpeed, carNum);
            }

        }

    }

    public void RelocateSecondCar()
    {
        var shelter = targetShelter.gameObject.GetComponent<ShelterStructure>();
        shelter.RelocateCarToShelter();
    }

    public void RelocateHorses()
    {

    }
    public void ApplyHomeHardening(float homeHardeningMod)
    {
        foreach(var house in sameTypeHouses)
        {
            house.HomeHardeningBehavior(homeHardeningMod);
        }
        
    }

    
    void HomeHardeningBehavior(float homeHardeningMod)
    {
        if (currentHouseModel == null) { Debug.Log("No house Model"); return; }
        currentHouseModel.material = metalRoofMaterial;
        combustible.fireChance = 1 - homeHardeningMod;
        Debug.Log("Fire Chance After Home Hardening: " + combustible.fireChance);
    }
}
