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
    private string selectedCatalogName;
    public FYT_evac car;
    public AudioSource goodSFX;
    public GameObject RadioBtn;
    private bool closePopup = false;
    private float timer = 0.12f;
    public Image panel;

    private AudioSource itemAudio;

    private void Start()
    {
        itemAudio = gameObject.AddComponent<AudioSource>();
    }

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
            selectedCatalogName = (StringManager.Instance != null)
                ? StringManager.Instance.GetEnglishText(selected.name)
                : selected.name;
            itemText.text = selectedDisplayName;
            if (FYT_ItemCatalog.IsKey(selectedCatalogName))
            {
                car.hasKey = true;
            }

            PlayItemAudio(selected.name);
        }
    }

    public void TakeItem()
    {
        itemAudio.Stop();

        FYT_Bag bag = GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>();
        bag.AddItem(selectedCatalogName);
        //bag.AddItem(selected.name);
        //bag.AddCatalogItem(selectedCatalogName);

        bool essential = FYT_ItemCatalog.GetTier(selectedCatalogName) == FYT_ItemCatalog.ItemTier.Essential;

        if (essential)
        {
            goodSFX.Play();
            Instantiate(Resources.Load("pickupFX_good"), selected.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("pickupFX"), selected.transform.position, Quaternion.identity);
        }

        if (FYT_ItemCatalog.IsKey(selectedCatalogName))
        {
            car.hasKey = true;
        }

        if (FYT_ItemCatalog.EnablesRadio(selectedCatalogName))
        {
            RadioBtn.SetActive(true);
        }

        Destroy(selected);
        closePopup = true;
    }

    public void LeaveItem()
    {
        itemAudio.Stop();
        closePopup = true;
    }

    private void PlayItemAudio(string key)
    {
        if (!TTSManager.IsEnabled || StringManager.Instance == null) return;

        string audioPath = StringManager.Instance.GetAudioPath(key);
        if (!string.IsNullOrEmpty(audioPath))
        {
            AudioClip clip = Resources.Load<AudioClip>(audioPath);
            if (clip != null)
            {
                itemAudio.volume = TTSManager.Volume;
                itemAudio.clip = clip;
                itemAudio.Play();
            }
            else
            {
                Debug.LogWarning($"Item audio clip not found: {audioPath}");
            }
        }
    }
}
