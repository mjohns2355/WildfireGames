using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACT_UIController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject learnMorePanel;
    //public HouseInfo currentHouseInfo;
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement;
    public Button placeRoadButton, placeHouseButton, placeSpecialButton, constructionButton;
    public GameObject buildingMenu;
    List<Button> buttonList;
    public List<HouseStructure> selectedHouses = new List<HouseStructure> ();
    public ShelterStructure selectedShelter;
    public List<Sprite> iconList;
    public List<StructureContextMenu> contextMenus = new List<StructureContextMenu>();
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

    public void AddMenu(StructureContextMenu menu)
    {
        if (!contextMenus.Contains(menu))
        {
            contextMenus.Add(menu);
        }
    }

    public void ClampToWindow( RectTransform panelRectTransform, float offset)
    {
        Vector3[] corners = new Vector3[4];
        panelRectTransform.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        // Padding from screen edges
        Vector3 adjustedPosition = panelRectTransform.position;

        if (bottomLeft.x < offset)
        {
            adjustedPosition.x += offset - bottomLeft.x;
        }
        if (topRight.x > Screen.width - offset)
        {
            adjustedPosition.x -= topRight.x - (Screen.width - offset);
        }
        if (bottomLeft.y < offset)
        {
            adjustedPosition.y += offset - bottomLeft.y;
        }
        if (topRight.y > Screen.height - offset)
        {
            adjustedPosition.y -= topRight.y - (Screen.height - offset);
        }

        panelRectTransform.position = adjustedPosition;
    }
}
