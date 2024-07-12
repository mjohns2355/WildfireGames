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
    string currentOption;
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
        //GameManager.Instance.uiController.AddSelectedHouse(this);
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

        //structureInfoDict["Car(s)"] = carNum;

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
        if(button.isGoodOption) return;
        OptionBehaviour();
        

        
    }

    void OptionBehaviour()
    {
        if(currentOption == null) return;
        switch (currentOption)
        {
            case " Wait for Notice ":
                waitSeconds = 2;
                break;
            case " Leave One Car ":
                if (carNum > 1)
                {
                    carNum--;
                }
                
                break;
            
        }
    }

    public IEnumerator SpawnCarRoutine()
    {

        Debug.Log("Wait for sim to start");
        yield return new WaitUntil(() => { return GameManager.Instance.startSim; });
        yield return new WaitForSeconds(waitSeconds);
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
