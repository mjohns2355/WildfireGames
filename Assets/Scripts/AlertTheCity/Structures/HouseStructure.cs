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
    public int pplNum;
    public int petNum;
    int carNum = 1;
    ATC_PlacementManager placementManager;
    public override void Awake()
    {
        base.Awake();
        //houseType = HouseType.none;
        

    }

    private void Start()
    {
        placementManager = GameManager.Instance.structureManager.placementManager;
        
        if (isMainHouse)
        {
            // only main house has info
            info = new HouseInfo(houseType);
            //info.gameObject.SetActive(true);
            menu.icon.SetActive(true);
            //info.SetIconFor(houseType);
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
                }
            }
        }

        //GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        //Instantiate(houseModel, transform.position, Quaternion.identity, transform);
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



#if UNITY_EDITOR
    private void OnEnable()
    {
        if (mesh.childCount >= 1) return;
        GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        Instantiate(houseModel, transform.position, Quaternion.identity, mesh);
    }

#endif


}
