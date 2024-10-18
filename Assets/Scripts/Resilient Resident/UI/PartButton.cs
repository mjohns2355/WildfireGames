using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    // Start is called before the first frame update

    public void InitPartUI(HousePart partInfo)
    {
        nameText.text = partInfo.name;
        priceText.text = partInfo.price.ToString();
    }
}
