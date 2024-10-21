using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorePanel : MonoBehaviour
{
    public TextMeshProUGUI typeCategoryText;
    public TextMeshProUGUI playerInfoText;
    public GameObject available;
    public GameObject shopPartIcon;
    

    public void ShowStorePanel(BaseHousePartObject targetHouseObj)
    {
        var partInfo = targetHouseObj.PartInfo;
        typeCategoryText.text = "Store: " + partInfo.housePartType.ToString().ToUpper();
        playerInfoText.text = $"{targetHouseObj.houseManager.playerTag}: ${targetHouseObj.houseManager.budegt.ToString()}";
    }
}
