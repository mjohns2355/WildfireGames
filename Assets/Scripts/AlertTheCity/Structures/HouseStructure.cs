//using System;
using System;
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
    List<ATC_StructureModel> destinations;
    float spawnCarChance = 0.9f;
    Dictionary<HouseType, List<HouseStructure>> houseTypeDict;
    Dictionary<StructureType, ATC_StructureModel> specialStructureDict;
    private void Awake()
    {

    }

    private void Start()
    {
        combustible =  GetComponent<Combustible>();
        placementManager = GameManager.Instance.structureManager.placementManager;
        carSpawnWaitTime = GameManager.Instance.fireManager.fireWaitTimeBeforeStart;

        if (!isMainHouse) return;
        InitMainHouse();

        // defaults to first option if player doesn't select
        currentOption = houseInfo.normalChoices.FirstOrDefault()?.choiceName;
        menu.onOptionSelected += OnOptionButtonClicked;

        InitDestinations();
        //default destination
        var shelter =specialStructureDict[StructureType.Shelter];
        SetDestination(new List<ATC_StructureModel> { shelter});
        targetShelter = shelter;
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
        GameObject houseModel = houseModels[UnityEngine.Random.Range(0, houseModels.Length)];
        currentHouseModel = Instantiate(houseModel, transform.position, mesh.transform.rotation, mesh).GetComponentInChildren<MeshRenderer>();
    }

    void InitMainHouse()
    {
       

        // only main house has info
        menu.gameObject.SetActive(true);
        menu.icon.gameObject.SetActive(true);
        ATC_UIController.Instance.AddMenu(menu);

        if (houseTypeDict == null)
        {
            houseTypeDict = new Dictionary<HouseType, List<HouseStructure>>();

            foreach (var house in placementManager.GetAllHouses())
            {
                if (house == null) continue;

                var houseStructure = house.GetComponent<HouseStructure>();
                if (houseStructure == null) continue;

                if (!houseTypeDict.ContainsKey(houseStructure.houseType))
                {
                    houseTypeDict[houseStructure.houseType] = new List<HouseStructure>();
                }

                houseTypeDict[houseStructure.houseType].Add(houseStructure);
            }
        }

        if (houseTypeDict.TryGetValue(houseType, out sameTypeHouses))
        {
            foreach (var house in sameTypeHouses)
            {
                house.houseInfo = houseInfo;
                house.houseInfo.InitHouseInfo(house);
            }
        }
    }
    void InitDestinations()
    {
        if (specialStructureDict == null)
        {
            specialStructureDict = new Dictionary<StructureType, ATC_StructureModel>();
            StructureType[] values = (StructureType[])Enum.GetValues(typeof(StructureType));
            foreach ( var type in values)
            {
                // house is not a special structure
                if (type == StructureType.House) continue;
                specialStructureDict[type] = placementManager.GetRandomSpecialStructursOfType(type);
            }
        }
    }
    public void RandomizeHouseType()
    {
        // 0 is None
        houseType = (HouseType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(HouseType)).Length);
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
            ATC_UIController.Instance.AddSelectedHouse(house);
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
        currentOption = menu.CurrentOption;
        Debug.Log($"Player selected {currentOption}");
        // apply home hardening immediately
        if(currentOption == "Home Hardening")
        {
            ApplyChoice();
        }
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
        if(currentOption != "Home Hardening")
        {
            ApplyChoice();
        }
        
        yield return new WaitForSeconds(carSpawnWaitTime);

        Debug.Log("After " + carSpawnWaitTime + "sec(s), " + houseType + " Spawned " + carNum + " " + carSpeed + " speed car(s)");
        //destination shelter

        foreach (var house in sameTypeHouses)
        {
            if (UnityEngine.Random.Range(0f, 1f) < spawnCarChance)
            {
                if (HasKidsToPickUp)
                {
                    var school = specialStructureDict[StructureType.School];
                    var shelter = specialStructureDict[StructureType.Shelter];
                    SetDestination(new List<ATC_StructureModel> { school, shelter });
                    ATC_AIDirector.Instance.SpawnCarWithMultipleStops(house.GetComponent<ATC_StructureModel>(),destinations, carSpeed, carNum);
                }
                else
                {
                    ATC_AIDirector.Instance.SpawnACar(house.GetComponent<ATC_StructureModel>(), destinations.Last(), carSpeed, carNum);
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
        var stable = specialStructureDict[StructureType.Stable];
        stable.GetComponent<StableStructure>().RelocateHorse();
        
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

    void SetDestination(List<ATC_StructureModel> destinations)
    {
        this.destinations = destinations;
    }
}
