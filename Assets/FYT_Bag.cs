using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYT_Bag : MonoBehaviour
{
    public GameObject bagPanel;
    public TextMeshProUGUI bagList;
    public TextMeshProUGUI bagList2;
    public TextMeshProUGUI bagList3;
    private string list;
    private string list2;
    private string list3;
    private int count = 0;

    public void OpenBag()
    {
        bagPanel.SetActive(true);
    }

    public void AddItem(string item)
    {
        count++;
        if (count <= 23)
        {
            list += "\n" + item;
            bagList.text = "Packed:" + list;
        } else if(count <= 46)
        {

            list2 += "\n" + item;
            bagList2.text = list2;
        } else
        {

            list3 += "\n" + item;
            bagList3.text = list3;
        }
    }
}
