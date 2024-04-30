using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACT_UIController : MonoBehaviour
{
    public GameObject canvas;
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement;
    public Button placeRoadButton, placeHouseButton, placeSpecialButton, constructionButton;
    public GameObject buildingMenu;
    List<Button> buttonList;
    public List<HouseStructure> selectedHouses = new List<HouseStructure> ();
    public ShelterStructure selectedShelter;
    private void Start()
    {
        buildingMenu.SetActive(false);
        buttonList = new List<Button> { placeRoadButton, placeHouseButton, placeSpecialButton };
        placeRoadButton.onClick.AddListener(() =>
        {
            OnRoadPlacement?.Invoke();
        });
        placeHouseButton.onClick.AddListener(() =>
        {
            OnHousePlacement?.Invoke();
        });
        placeSpecialButton.onClick.AddListener(() =>
        {
            OnSpecialPlacement?.Invoke();
        });
    }

    public void UpdateConstructionMode(bool state)
    {
        Text text = constructionButton.gameObject.GetComponentInChildren<Text>();
        if (state == true)
        {
            text.text = "Construction ON";
        }
        else
        {
            text.text = "Construction OFF";
        }
        buildingMenu.SetActive(state);
    }

    public void AddSelectedHouse(HouseStructure house)
    {
        selectedHouses.Add(house);
    }

    public void RemoveSelectedStructure(HouseStructure house)
    {
        selectedHouses.Remove(house);
    }
}
