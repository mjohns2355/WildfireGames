using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiddenItems : MonoBehaviour
{

    public TMPro.TextMeshProUGUI bagText;

    public GameObject[] backpack;
    private int itemCount = 0;


    public void collect(string item)
    {
        if(itemCount < 4)
        {
            backpack[itemCount].SetActive(false);
            itemCount++;
            backpack[itemCount].SetActive(true);
            bagText.text += "\n" + item;
            GameObject.Find(item).SetActive(false);
        } else if(itemCount < 5)
        {
            itemCount++;
            bagText.text = "*Bag is Full*\n" + bagText.text;
        }
    }
}
