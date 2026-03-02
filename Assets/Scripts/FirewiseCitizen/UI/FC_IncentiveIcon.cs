using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FC_Incentiveicon : MonoBehaviour
{
    public Image icon;
    public Button offerButton, iconButton;

    HouseStructure owner;
    
    // Start is called before the first frame update
    void Start()
    {
        if (StringManager.Instance != null)
        {
            var btnText = offerButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = StringManager.Instance.GetText("offerText");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpIcon(HouseStructure owner, Sprite icon)
    {
        this.owner = owner;
        this.icon.sprite = icon;
    }

}
