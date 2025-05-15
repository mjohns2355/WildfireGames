//using System;
using System;
using System.Buffers;
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
    public HouseType houseType;
    //[SerializeField] HouseType houseType;
    //[SerializeField] GameObject[] houseModels;
    [SerializeField] Transform mesh,front;
    [SerializeField] Material metalRoofMaterial;

    public int petNum = 0;
    public int carNum = 1;
    public int horseNum = 0;
    public int kidNum = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public float carSpawnWaitTime = 0f;
    public float homeHardeningChance;
    public float followOrderChance = 0.5f;
    //public bool HasHorseTrailers
    //{
    //    get { return houseType == HouseType.horse && horseNum != 0; }
    //    set { HasHorseTrailers = value; }
    //}

    public bool HasKidsToPickUp
    {
        get { return houseType == HouseType.kids && kidNum != 0; }
        set { HasKidsToPickUp = value; }
    }


    List<HouseStructure> sameTypeHouses = new List<HouseStructure>();
    //List<HouseChoice> choices = new List<HouseChoice>();
    string lastOption = string.Empty;
    string currentOption;
    ATC_PlacementManager placementManager;
    Combustible combustible;
    [SerializeField] List<MeshRenderer> currentHouseModels;
    ATC_StructureModel targetShelter;
    List<ATC_StructureModel> destinations;

    bool followedOrder = false;

    private void Awake()
    {
        houseInfo = null;
    }
    public override void Start()
    {
        base.Start();
        combustible =  GetComponent<Combustible>();
        placementManager = GameManager.Instance.structureManager.placementManager;
        carSpawnWaitTime = GameManager.Instance.fireManager.fireWaitTimeBeforeStart;
       
        combustible.OnIgnite.AddListener(CheckNeighbourRoad);
        ModifyStructureRotation();

        //if (!isMainHouse) return;
        //InitMainHouse();





    }

    //private void OnEnable()
    //{
    //    SpawnHouseModel();
    //}

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.F) && testHouse)
        //{
        //    TestSpawnCar();
        //}
        
    }

    void CheckNeighbourRoad()
    {
        var pos = Vector3Int.RoundToInt(transform.position);

        var nearbyRoads = placementManager.GetNeighbourOfTypesFor(pos, CellType.Road);

        foreach (var road in nearbyRoads)
        {
            placementManager.SetCostFor(road, 10f);
        }

    }
    //void TestSpawnCar()
    //{
    //    var targetShelter = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.Shelter);
    //    var targetSchool= GameManager.Instance.structureManager.placementManager.GetRandomSpecialStructursOfType(StructureType.School);
    //    //ATC_AIDirector.Instance.SpawnCarWithMultipleStops(GetComponent<ATC_StructureModel>(), new List<ATC_StructureModel> { targetSchool, targetShelter }, CarSpeed.fast, 1);
    //    ATC_AIDirector.Instance.SpawnACar(GetComponent<ATC_StructureModel>(), targetShelter, CarSpeed.fast, 1);
    //}
    public void SpawnHouseModel()
    {
        if (mesh.childCount >= 1) return;
        //GameObject houseModel = houseModels[UnityEngine.Random.Range(0, houseModels.Length)];
        GameObject houseModel = GameManager.Instance.structureManager.GetHouseModel(houseType);
        if (!houseModel)
        {
            Debug.Log("House Model is null, please check the prefab in the resource manager");
            return;
        }
        currentHouseModels = Instantiate(houseModel, transform.position, mesh.transform.rotation, mesh).GetComponentsInChildren<MeshRenderer>().ToList();
    }

    public void InitMainHouse()
    {
        //Debug.Log("Init Main House");
        // only main house has info
        contextMenu.gameObject.SetActive(true);
        //var isFirstSim = GameManager.Instance.IsFirstSim;
        //contextMenu.icon.gameObject.SetActive(!isFirstSim);
        contextMenu.icon.gameObject.SetActive(true);
        ATC_UIController.Instance.AddMenu(contextMenu);
        var model = GetComponent<ATC_StructureModel>();
        roadPosition = model.RoadPosition;

        if (GameManager.Instance.structureManager.houseTypeDict.TryGetValue(houseType, out sameTypeHouses))
        {
            foreach (var house in sameTypeHouses)
            {
                house.houseInfo = houseInfo;
                house.houseInfo.InitHouseInfo(house);
                var m = house.GetComponent<ATC_StructureModel>();
                house.roadPosition = m.RoadPosition;
            }
        }
        // defaults option if player doesn't select
        currentOption = houseInfo.defaultChoice.choiceName;
        contextMenu.onOptionConfirmed += OnOptionConfirmed;
        ATC_UIController.Instance.icons.Add(contextMenu.icon);
        //choices = GameManager.Instance.structureManager.GetPlayerChoicesDict()[HouseType];
        //InitSpecialStructDict();
        //default destination
        var shelter = GameManager.Instance.structureManager.specialStructureDict[StructureType.Shelter];
        SetDestination(new List<ATC_StructureModel> { shelter });
        targetShelter = shelter;
        //wui house has very low chance to follow order at the beginning
        followOrderChance = houseType == HouseType.wui ? 0.2f : followOrderChance;
    }

    public void RandomizeHouseType()
    {
        // 0 is None
        var types = GameManager.Instance.availableHouseTypes;
        houseType = types[UnityEngine.Random.Range(0,types.Count-1)];
    }

    public void SetHouseType(HouseType type)
    {
        houseType = type;

    }
    public override void OnStructureClick()
    {
        if (isMainHouse && GameManager.Instance.currentStage != LevelStage.HouseDialog)
        {
            contextMenu.OnMainHouseClicked();
            //foreach (var house in sameTypeHouses)
            //{
            //    house.outline.enabled = true;
            //    ATC_UIController.Instance.AddSelectedHouse(house);
            //}
        }
    }

    public override void StopSturctureClick()
    {
        foreach (var house in sameTypeHouses)
        {
            //house.outline.enabled=false;
            house.SetOutline(false);
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



    void OnOptionConfirmed()
    {
        foreach (var option in contextMenu.selectedOptions)
        {
            currentOption = option.GetOptionContent();
            Debug.Log($"Player selected {currentOption}");
            var currentChoice = GetCurrentChoice(currentOption);
            // home hardening should apply immediately
            //if(currentChoice.choiceName == "Evacuate Early & Home Hardening")
            //{
            //    currentChoice.ApplyHomeHardening(this);
            //    ApplyHomeHardeningToAllHouses(currentChoice.homeHardeningMod);
            //}

            if (currentChoice != null)
            {
                GameManager.Instance.structureManager.UpdatePlayerChoicesDict(houseType, currentChoice);
                Debug.Log($"Updated Player Choices Dict ({houseType}, {currentChoice.choiceName})");
            }
        }
    }

    void ApplyChoice()
    {
        var otherHousesRng = UnityEngine.Random.Range(0, 1f);
        foreach (var currentChoice in GameManager.Instance.structureManager.GetPlayerChoicesDict()[houseType])
        {
            //var currentChoice = GetCurrentChoice(currentOption);
     
            if (!currentChoice.isNormal)
            {
                // wui house has vary low chance to follow order at the beginning
                // each wui house has individual chance to follow order
                float rng = houseType == HouseType.wui ? UnityEngine.Random.Range(0f, 1f) : otherHousesRng;

                if (rng < followOrderChance)
                {
                    ApplyChoiceEffect(houseInfo.defaultChoice);
                    followedOrder = false;
                    GameManager.Instance.UpdateHouseResponse(houseType, "Disregarded");
                    return;
                }
                else
                {
                    followedOrder = true;
                    GameManager.Instance.UpdateHouseResponse(houseType, "Followed");
                }
            }
            else
            {
                GameManager.Instance.UpdateHouseResponse(houseType, "Followed");
            }
            ApplyChoiceEffect(currentChoice);
            
        }

    }

    void ApplyChoiceEffect(HouseChoice choice)
    {
        foreach (var house in sameTypeHouses)
        {
            //Debug.Log($"{house.houseType} applies {choice.choiceName},car num mod: {choice.carNumberMod}");
            choice.ApplyEffect(house);
        }

        if (isMainHouse)
        {
            choice.ApplyEffect(this);
            choice.ApplySpecialEffect(this);
        }
        Debug.Log($"{houseType} decides to {choice.choiceName}");
    }
    HouseChoice GetCurrentChoice(string name)
    {
        return houseInfo.ReturnChoiceByName(name).choice;
    }
    public IEnumerator SpawnCarRoutine()
    {
        ApplyChoice();
        //outline.enabled = false;
        SetOutline(false);
        yield return new WaitForSeconds(carSpawnWaitTime);

        //Debug.Log("After " + carSpawnWaitTime + "sec(s), " + houseType + " Spawned " + carNum + " " + carSpeed + " speed car(s)");
        //make sure main house also spawn car
        SpawnCar(this);
        foreach (var house in sameTypeHouses)
        {
            SpawnCar(house);
        }
    }

    private void SpawnCar(HouseStructure house)
    {
        if (UnityEngine.Random.Range(0f, 1f) < GameManager.Instance.spawnCarChance)
        {
            if (HasKidsToPickUp && followedOrder)
            {
                var school = GameManager.Instance.structureManager.specialStructureDict[StructureType.School];
                var shelter = GameManager.Instance.structureManager.specialStructureDict[StructureType.Shelter];
                SetDestination(new List<ATC_StructureModel> { school, shelter });
                //Debug.Log("Added School to destinations");  
                ATC_AIDirector.Instance.SpawnCarWithMultipleStops(house.GetComponent<ATC_StructureModel>(), destinations, carSpeed, carNum);
            }
            else
            {
                ATC_AIDirector.Instance.SpawnACar(house.GetComponent<ATC_StructureModel>(), destinations.Last(), carSpeed, carNum);
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
        var stable = GameManager.Instance.structureManager.specialStructureDict[StructureType.Stable];
        stable.GetComponent<StableStructure>().RelocateHorse();
        
    }
    public void ApplyHomeHardeningToAllHouses(float homeHardeningMod)
    {
        foreach(var house in sameTypeHouses)
        {
            var rng = UnityEngine.Random.Range(0f, 1f);
            if(rng < homeHardeningChance)
            {
                house.HomeHardeningBehavior(homeHardeningMod);
            }
            else
            {
                Debug.Log("Home Hardening Failed");
            }

        }
        
    }

    public void OnReceivedIncentives()
    {
        followOrderChance = 1f;
    }
    void HomeHardeningBehavior(float homeHardeningMod)
    {
        if (currentHouseModels.Count == 0) { Debug.Log("No house Model"); return; }
        Debug.Log("Apply Home Hardening");
        foreach(var model in currentHouseModels)
        {
            model.material = metalRoofMaterial;
        }
        combustible.fireChance = 1 - homeHardeningMod;
        //Debug.Log("Fire Chance After Home Hardening: " + combustible.fireChance);
    }

    void SetDestination(List<ATC_StructureModel> destinations)
    {
        this.destinations = destinations;
    }


}
