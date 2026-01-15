using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    public bool hasCat = false;
    public bool hasDocs = false;
    public bool hasMask = false;
    public GameObject siren;
    public GameObject timer;

    public void OpenBag()
    {
        bagPanel.SetActive(true);
    }

    public void Evac()
    {
        Destroy(siren);
        Destroy(timer);
        /*
        bagList2.text = "";
        bagList.text = "Good job evacuating quickly!";
        if (hasCat)
        {
            bagList.text += "\nYour friend is relieved you got the cat out safely.";
        }
        else
        {
            bagList.text += "\nYour friend is distraught that you could not get to the cat, but upon returning to the house later they found the cat had survived.";
        }
        if (hasDocs)
        {
            bagList.text += "\nYour friend is grateful that important documents were retrieved.";
        }
        else
        {
            bagList.text += "\nSome important documents were lost to the fire.";
        }
        if (hasMask)
        {
            bagList.text += "\nIt was a good idea to take protective equipment for yourself, like the mask to protect from smoke.";
        }
        bagList3.text = "";
        */
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void AddItem(string item)
    {
        if (!bagList.text.Contains(item) && !bagList2.text.Contains(item) && !bagList3.text.Contains(item))
        {
            count++;

            string translatedItem = (StringManager.Instance != null) ? StringManager.Instance.GetText(item) : item;
            //packedText
            

            if (count <= 18)
            {
                list += "\n" + translatedItem;
                bagList.text = "Packed:" + list;
            }
            else if (count <= 36)
            {

                list2 += "\n" + translatedItem;
                bagList2.text = list2;
            }
            else
            {

                list3 += "\n" + translatedItem;
                bagList3.text = list3;
            }
        }
    }
}
