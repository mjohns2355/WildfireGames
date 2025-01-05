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
    public AudioSource goodSFX;
    public GameObject RadioBtn;
    private bool closePopup = false;
    private float timer = 0.12f;

    private void Update()
    {
        if (closePopup)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                popup.SetActive(false);
                timer = 0.12f;
                closePopup = false;
            }
        }
    }

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
        if(selected.name == "Salem the Cat"){
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasCat = true;
            goodSFX.Play();
        }
        else if (selected.name == "Important Documents" || selected.name == "Salem's Vet Records")
        {
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasDocs = true;
            goodSFX.Play();
        }
        else if (selected.name == "N95 Mask")
        {
            GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().hasMask = true;
            goodSFX.Play();
        }
        else if (selected.GetComponent<FYT_collectable>().isKey)
        {
            car.hasKey = true;
            goodSFX.Play();
        } else if(selected.name == "Radio")
        {
            RadioBtn.SetActive(true);
        }
        Destroy(selected);
        closePopup = true;
    }

    public void LeaveItem()
    {

        closePopup = true;
    }
}
