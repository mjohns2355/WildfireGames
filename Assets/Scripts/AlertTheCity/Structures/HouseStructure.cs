using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[ExecuteInEditMode]
public enum HouseType { none, elderly, twoCar, kids, horse, pet, wui }
public class HouseStructure : Structure
{
    public bool isMainHouse;
    public HouseInfo info;
    public HouseType houseType;
    [SerializeField] List<HouseStructure> sameTypeHouses = new List<HouseStructure>();
    [SerializeField] GameObject[] houseModels;
    [SerializeField] Transform mesh;
    [SerializeField] int petNum = 0;
    [SerializeField] int carNum = 0;
    [SerializeField] int horseNum = 0;
    [SerializeField] CarSpeed carSpeed;
    [SerializeField] float waitSeconds = 0f;
    //public int petNum;
    ATC_PlacementManager placementManager;
    

    private void Start()
    {
        placementManager = GameManager.Instance.structureManager.placementManager;
        
        if (isMainHouse)
        {
            // only main house has info
            info = new HouseInfo(houseType);
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
                    houseStructure.carNum = info.carNumber;
                    houseStructure.petNum = info.petNumber;
                    houseStructure.carSpeed = info.carSpeed;
                }
            }
            menu.onOptionSelected += OnOptionButtonClicked;
        }

        //GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        //Instantiate(houseModel, transform.position, Quaternion.identity, transform);
    }

    private void OnEnable()
    {
        if (mesh.childCount >= 1) return;
        GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        Instantiate(houseModel, transform.position, Quaternion.identity, mesh);
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
        //GameManager.Instance.uiController.AddSelectedHouse(this);
    }

    public void AfterSpawnACar()
    {
        if (carNum <= 0) return;

        carNum--;

        //structureInfoDict["Car(s)"] = carNum;

    }

    public bool CanSpawnCar()
    {

        return carNum > 0;
        
    }

    

    public override void StopSturctureClick()
    {
        //menu.gameObject.SetActive(false);
        foreach (var house in sameTypeHouses)
        {
            house.outline.enabled = false;
            GameManager.Instance.uiController.RemoveSelectedStructure(house);
        }
    }


    void OnOptionButtonClicked(OptionButton button)
    {
        
    }

    void OptionBehaviour(string option)
    {
        switch (option)
        {
            case "Wait for Notice":
                
                waitSeconds = 2;
                break;
            case "Leave One Car":
                
                break;
            
        }
    }

    public IEnumerator SpawnCarRoutine()
    {
        Debug.Log("Wait for sim to start");
        yield return new WaitUntil(() => { return GameManager.Instance.startSim; });
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
