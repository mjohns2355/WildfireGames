using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PartButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    private UnityEngine.UI.Outline outline;
    private HousePartInfo partInfo;
    private Button button;
    // Start is called before the first frame update

    private void Start()
    {
       outline = GetComponent<UnityEngine.UI.Outline>();
       button = GetComponent<Button>();
       button.onClick.AddListener(OnShopPartIconClicked);
    }

    public void InitPartIconButton(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        nameText.text = partInfo.partID;
        priceText.text = $"$ {partInfo.price}" ;
    }

    void OnShopPartIconClicked()
    {
        HH_GameManager.Instance.UIManager.ShowPurchasePopup(partInfo);
    }
}
