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
    public TextMeshProUGUI nameText;
    public Button button;
    public GameObject checkMark;
    // Start is called before the first frame update

    public void InitPlantMenuOption (BaseCombustibleInfo combustibleInfo, bool isBought)
    {
        icon.sprite = combustibleInfo.icon;
        nameText.text = combustibleInfo.partID;
        checkMark.SetActive(isBought);
    }
}
