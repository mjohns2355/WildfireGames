using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYTPickUp : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI itemText;
    private GameObject selected;

    public void OpenPopup(GameObject g)
    {
        popup.SetActive(true);
        selected = g;
        itemText.text = selected.name;
    }

    public void TakeItem()
    {
        GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().AddItem(selected.name);
        Destroy(selected);
    }
}
