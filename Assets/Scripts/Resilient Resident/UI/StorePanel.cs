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

    private HousePartType targetCategory;

    public void ShowStorePanel(BaseHousePartObject targetHouseObj)
    {
        var partInfo = targetHouseObj.PartInfo;
        var player = HH_GameManager.Instance.currentPlayer;
        targetCategory = partInfo.housePartType;
        typeCategoryText.text = "Store: " + partInfo.housePartType.ToString().ToUpper();
        playerInfoText.text = $"{player.playerTag}: ${player.budegt.ToString()}";

        PopulateIconsInStore();
    }

    void PopulateIconsInStore()
    {
        foreach( var p in ResourceManager.Instance.allAvailableParts[targetCategory])
        {
            var icon = Instantiate(shopPartIcon,available.transform).GetComponent<PartButton>();
            icon.InitPartIconButton(p);
        }
    }
}
