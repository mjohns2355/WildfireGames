using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FC_Incentiveicon : MonoBehaviour
{
    public Image icon;
    public Button offerButton;

    HouseStructure owner;
    
    // Start is called before the first frame update
    void Start()
    {
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
