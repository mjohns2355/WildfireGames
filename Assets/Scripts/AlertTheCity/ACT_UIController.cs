using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACT_UIController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject learnMorePanel;
    public HouseInfo currentHouseInfo;
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

    public void ClampToWindow(Vector3 uiPos, RectTransform panelRectTransform, RectTransform parentRectTransform)
    {

        panelRectTransform.transform.position = uiPos;

        Vector3 pos = panelRectTransform.localPosition;

        Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
        Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

        pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

        panelRectTransform.localPosition = pos;
    }
}
