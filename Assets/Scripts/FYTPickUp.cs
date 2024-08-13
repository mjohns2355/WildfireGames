using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYTPickUp : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI itemText;
    private GameObject selected;
    public FYT_evac car;

    public void OpenPopup(GameObject g)
    {
        if (!popup.activeInHierarchy)
        {
            popup.SetActive(true);
            selected = g;
            itemText.text = selected.name;
            if (g.GetComponent<FYT_collectable>().isKey)
            {
                car.hasKey = true;
            }
        }
    }

    public void TakeItem()
    {
        GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().AddItem(selected.name);
        if(selected.name == "Cat"){
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasCat = true;
        }
        else if (selected.name == "Important Documents")
        {
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasDocs = true;
        }
        else if (selected.name == "N95 Mask")
        {
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasMask = true;
        }
        else if (selected.GetComponent<FYT_collectable>().isKey)
        {
            car.hasKey = true;
        }
        Destroy(selected);
    }
}
