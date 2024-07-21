using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//[ExecuteInEditMode]


public enum HouseType { none, elderly, twoCar, kids, horse, pet, wui }
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
    public int petNum = 0;
    public int carNum = 1;
    public int horseNum = 0;
    public int kidNum = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public float spawnTime = 0f;
    public float homeHardening = 0f;
    string lastOption = string.Empty;
    string currentOption = "Wait for Notice";
    //public int petNum;
    ATC_PlacementManager placementManager;
    [SerializeField]Combustible combustible;
    [InspectorButton("ApplyChoice")]
    public bool Apply;
    private void Start()
    {
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
                var houseStructure = house.GetComponentInChildren<HouseStructure>();
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
            GameManager.Instance.structureManager.allMainHouses.Add(this);
        }

        //GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        //Instantiate(houseModel, transform.position, Quaternion.identity, transform);
    }

    private void OnEnable()
    {
        if (mesh.childCount >= 1) return;
        GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        Instantiate(houseModel, transform.position, mesh.transform.rotation, mesh);
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
                if(lastOption == currentOption) return;
                choice.ApplyEffect(this);
                lastOption = currentOption;
            }
        }
        

    }
    public IEnumerator SpawnCarRoutine()
    {
        ApplyChoice();
        yield return new WaitForSeconds(spawnTime);
        Debug.Log("Spawned " + carNum + " cars");
        //destination shelter
        var shelter = GameManager.Instance.structureManager.placementManager.GetRandomSpecialStrucutre();
        foreach (var house in sameTypeHouses)
        {
            ATC_AIDirector.Instance.SpawnACar(house.GetComponentInParent<ATC_StructureModel>(), shelter, carSpeed,carNum);
            
        }

 
    }

#if UNITY_EDITOR


#endif


}
