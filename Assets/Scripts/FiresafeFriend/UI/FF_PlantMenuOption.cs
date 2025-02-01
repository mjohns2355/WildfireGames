using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FF_PlantMenuOption : MonoBehaviour
{
    public Image icon;
    public Sprite removeIcon;
    public TextMeshProUGUI nameText;
    public Button button;
    public FF_Plants ownerPlant;
    public FF_DirtMound ownerMound;
    private bool isRemoveButton;

    //public GameObject checkMark;
    // Start is called before the first frame update
    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    public void InitPlantMenuOption (FF_Plants plant, FF_DirtMound mound)
    {
        ownerPlant = plant;
        ownerMound = mound;
        icon.sprite = plant.combustibleInfo.icon;
        nameText.text = plant.combustibleInfo.partID;
        //checkMark.SetActive(isBought);
    }

    public void InitRemoveButton(FF_DirtMound mound)
    {
        ownerMound = mound;
        isRemoveButton = true;
        nameText.text = "Remove Plant";
        icon.sprite = removeIcon;
    }
    public void OnClick()
    {
        if(isRemoveButton)
        {
            ownerMound.Shovel();
            return;
        }
        ownerMound.Plant(ownerPlant);
    }

    private void OnDestroy()
    {
       button.onClick.RemoveAllListeners();
    }
}
