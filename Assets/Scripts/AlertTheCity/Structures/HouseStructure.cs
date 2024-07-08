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
    public string houseInfo;
    [SerializeField] Image iconSprite;
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
            SetUpHouseInfoForType(houseType);
            iconSprite.gameObject.SetActive(true);
            iconSprite.sprite = ReturnIconForType(houseType);
            List<ATC_StructureModel> houses = placementManager.GetAllHouses();
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
        menu.gameObject.SetActive(false);
        foreach (var house in sameTypeHouses)
        {
            house.outline.enabled = false;
            GameManager.Instance.uiController.RemoveSelectedStructure(house);
        }
    }

    private void SetUpHouseInfoForType(HouseType houseType)
    {
        switch (houseType)
        {
            case HouseType.elderly:
                houseInfo = "Elderly House: Wait for family member | Ask for ride early";
                break;
            case HouseType.twoCar:
                houseInfo = "Two-car House: Take both cars | Leave one car behind | Relocate second car ";
                carNum = 2;
                break;
            case HouseType.horse:
                houseInfo = "Horse Owner: Wait for evac order | Relocate horses ";
                break;
            case HouseType.pet:
                houseInfo = "Pet Owner: Wait for evac order | Plan ahead ";
                break;
            case HouseType.wui:
                houseInfo = "WUI House: Wait for evac order | Evacuate early ";
                break;
            case HouseType.kids:
                houseInfo = "Kids House: Pick up from school | Plan ahead ";
                break;
        }
    }

    private Sprite ReturnIconForType(HouseType houseType) {
        foreach( var icon in GameManager.Instance.uiController.iconList)
        {
            if (icon.name == houseType.ToString())
            {
                return icon;
            }
        }

        return null;
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
