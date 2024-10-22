using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PartButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    [SerializeField] UnityEngine.UI.Outline outline;
    // Start is called before the first frame update

    private void Start()
    {
       outline = GetComponent<UnityEngine.UI.Outline>();
    }
    public void InitPartUI(HousePart partInfo)
    {
        nameText.text = partInfo.name;
        priceText.text = partInfo.price.ToString();
    }

    public void InitPartIconButton(HousePart partInfo)
    {
        nameText.text = partInfo.name;
        priceText.text = $"$ {partInfo.price}" ;
    }
}
