using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[ExecuteInEditMode]
public class HouseStructure : Structure
{
    public enum HouseType { elderly, twoCar, kids, horse, pet, wui }
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

        
        if (isMainHouse)
        {
           

        }
        //InitializeInfoDictionary();
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
                houseInfo = "Elderly House:Wait for family member | Ask for ride early";
                break;
            case HouseType.twoCar:
                houseInfo = "Two-car House:Take both cars | Leave one car behind | Relocate second car";
                carNum = 2;
                break;
            case HouseType.horse:
                houseInfo = "Horse Owner:Take both cars | Leave one car behind | Relocate second car";
                break;
            case HouseType.pet:
                houseInfo = "Pet Owner:Wait for evac order | Plan ahead ";
                break;
            case HouseType.wui:
                houseInfo = "WUI House:Take both cars | Leave one car behind | Relocate second car";
                break;
            case HouseType.kids:
                houseInfo = "Kids House:Take both cars | Leave one car behind | Relocate second car";
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
        if (mesh.childCount != 0) return;
        GameObject houseModel = houseModels[Random.Range(0, houseModels.Length)];
        Instantiate(houseModel, transform.position, Quaternion.identity, mesh);
    }

#endif


}
