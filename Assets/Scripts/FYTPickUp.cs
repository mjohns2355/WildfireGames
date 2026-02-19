using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FYTPickUp : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI itemText;
    private GameObject selected;
    private string selectedDisplayName;
    public FYT_evac car;
    public AudioSource goodSFX;
    public GameObject RadioBtn;
    private bool closePopup = false;
    private float timer = 0.12f;
    public Image panel;

    private void Update()
    {
        if (closePopup)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                popup.SetActive(false);
                panel.enabled = false;
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
            panel.enabled = true;
            selected = g;
            selectedDisplayName = (StringManager.Instance != null)
                ? StringManager.Instance.GetText(selected.name)
                : selected.name;
            itemText.text = selectedDisplayName;
            if (FYT_ItemCatalog.IsKey(selectedDisplayName))
            {
                car.hasKey = true;
            }
        }
    }

    public void TakeItem()
    {
        FYT_Bag bag = GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>();
        bag.AddItem(selected.name);

        bool essential = FYT_ItemCatalog.GetTier(selectedDisplayName) == FYT_ItemCatalog.ItemTier.Essential;

        if (essential)
        {
            goodSFX.Play();
            Instantiate(Resources.Load("pickupFX_good"), selected.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("pickupFX"), selected.transform.position, Quaternion.identity);
        }

        if (FYT_ItemCatalog.IsKey(selectedDisplayName))
        {
            car.hasKey = true;
        }

        if (FYT_ItemCatalog.EnablesRadio(selectedDisplayName))
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
