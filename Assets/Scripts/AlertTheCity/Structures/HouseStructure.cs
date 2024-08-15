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
    public HouseTypeInfo houseInfo;
    public bool testHouse;
    [SerializeField] HouseType houseType;
    [SerializeField] GameObject[] houseModels;
    [SerializeField] Transform mesh;
    [SerializeField] Material metalRoofMaterial;

    public int petNum = 0;
    public int carNum = 1;
    public int horseNum = 0;
    public int kidNum = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public float carSpawnWaitTime = 0f;
    public bool HasHorseTrailers
    {
        get { return houseType == HouseType.horse && horseNum != 0; }
        set { HasHorseTrailers = value; }
    }

    public bool HasKidsToPickUp
    {
        get { return houseType == HouseType.kids && kidNum != 0; }
        set { HasKidsToPickUp = value; }
    }
    public HouseType HouseType
    {
        get { return houseType; }
        private set { houseType = value; }
    }

    List<HouseStructure> sameTypeHouses = new List<HouseStructure>();
    List<HouseChoice> choices = new List<HouseChoice>();
    string lastOption = string.Empty;
    string currentOption;
    ATC_PlacementManager placementManager;
    Combustible combustible;
    [SerializeField] MeshRenderer currentHouseModel;
    ATC_StructureModel targetShelter;
    float spawnCarChance = 0.9f;

    private void Awake()
    {

    }

    private void Start()
    {
        combustible =  GetComponent<Combustible>();
        placementManager = GameManager.Instance.structureManager.placementManager;
        carSpawnWaitTime = GameManager.Instance.fireManager.fireWaitTimeBeforeStart;

        if (isMainHouse)
        {
            // only main house has info
            menu.gameObject.SetActive(true);
            menu.icon.gameObject.SetActive(true);
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
                        h.houseInfo = houseInfo;
                        h.houseInfo.InitHouseInfo(h);
                    }
                }
            }

            // defaults to first option if player doesn't select
            currentOption = houseInfo.normalChoices[0].choiceName;
            menu.onOptionSelected += OnOptionButtonClicked;
            targetShelter = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.Shelter);
        }
        
    }

    private void OnEnable()
    {
        SpawnHouseModel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && testHouse)
        {
            TestSpawnCar();
        }
    }

    void TestSpawnCar()
    {
        var targetShelter = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.Shelter);
        var targetSchool= GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.School);
        ATC_AIDirector.Instance.SpawnCarWithMultipleStops(GetComponent<ATC_StructureModel>(), new List<ATC_StructureModel> { targetSchool, targetShelter }, CarSpeed.fast, 1);
        //ATC_AIDirector.Instance.SpawnACar(GetComponent<ATC_StructureModel>(), targetShelter, CarSpeed.fast, 1);
    }
    void SpawnHouseModel()
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



    void OnOptionButtonClicked()
    {
        currentOption = menu.CurrentOption.GetOptionContent();
        Debug.Log($"Player selected {currentOption}");
        
    }

    void ApplyChoice()
    {
        var currentChoice = GetCurrentChoice(currentOption);

        if(currentChoice != null)
        {
            // avoid applying the same choices multiple times
            if (lastOption == currentOption) return;
            foreach (var house in sameTypeHouses)
            {
                currentChoice.ApplyEffect(house);
            }
            //choice.ApplyEffect(this);
            lastOption = currentOption;

            GameManager.Instance.structureManager.UpdatePlayerChoicesDict(houseType, currentChoice);
        }


        
        //choices = houseInfo.normalChoices.Union(houseInfo.lockedChoices).ToList();

        //foreach(var choice in choices)
        //{
        //    if(choice.choiceName == currentOption)
        //    {
        //        //Debug.Log("current option: " + currentOption);
        //        if(lastOption == currentOption) return;

        //        foreach (var house in sameTypeHouses)
        //        {
        //            choice.ApplyEffect(house);
        //        }
        //        //choice.ApplyEffect(this);
        //        lastOption = currentOption;
        //        break;
        //    }
        //    else
        //    {
        //        // default spawn time for house type doesn't have 'Wait for Notice' option
        //        carSpawnWaitTime = 5;
        //    }
        //}


    }

    HouseChoice GetCurrentChoice(string name)
    {
        choices = houseInfo.normalChoices.Union(houseInfo.lockedChoices).ToList();
        foreach (var choice in choices)
        {
            if (choice.choiceName == name)
            {
                return choice;
            }
        }

        return null;
    }
    public IEnumerator SpawnCarRoutine()
    {
        ApplyChoice();
        yield return new WaitForSeconds(carSpawnWaitTime);

        Debug.Log("After " + carSpawnWaitTime + "sec(s), " + houseType + " Spawned " + carNum + " " + carSpeed + " speed car(s)");
        //destination shelter

        foreach (var house in sameTypeHouses)
        {
            if (Random.Range(0f, 1f) < spawnCarChance)
            {
                if (HasKidsToPickUp)
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
    }

    public void RelocateSecondCar()
    {
        var shelter = targetShelter.GetComponent<ShelterStructure>();
        shelter.RelocateCarToShelter();
    }

    public void RelocateHorses()
    {
        var stable = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.Stable).GetComponent<StableStructure>();
        stable.RelocateHorse();
        
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
