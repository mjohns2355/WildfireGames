using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FF_PlantMenuOption : MonoBehaviour
{
    public Image icon;
    public Sprite removeIcon;
    public TextMeshProUGUI nameText, shortDescription;
    public Button button;
    public FF_Plants ownerPlant;
    public FF_DirtMound ownerMound;
    private bool isRemoveButton;

    public string fireResistantKey = "plant_material_fire_resistant";
    public string moderatelyFlammableKey = "plant_material_moderately_flammable";
    public string flammableKey = "plant_material_flammable";
    public string highlyFlammableKey = "plant_material_highly_flammable";
    public string removePlantKey = "plant_remove";

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void InitPlantMenuOption(FF_Plants plant, FF_DirtMound mound)
    {
        ownerPlant = plant;
        ownerMound = mound;

        icon.sprite = plant.combustibleInfo.icon;

        // Assuming partID is already localized OR is an ID you want as-is
        //nameText.text = plant.combustibleInfo.partID;
        if (StringManager.Instance != null)
        {
            nameText.text = StringManager.Instance.GetText(plant.combustibleInfo.partID);
        }
        else
        {
            nameText.text = plant.combustibleInfo.partID;
        }

        string descriptionKey = plant.combustibleInfo.materialClass switch
        {
            MaterialClass.A => fireResistantKey,
            MaterialClass.B => moderatelyFlammableKey,
            MaterialClass.C => flammableKey,
            MaterialClass.F => highlyFlammableKey,
            _ => null
        };

        if (!string.IsNullOrEmpty(descriptionKey))
        {
            shortDescription.text = StringManager.Instance.GetText(descriptionKey);
        }
        else
        {
            shortDescription.text = string.Empty;
        }
    }

    public void InitRemoveButton(FF_DirtMound mound)
    {
        ownerMound = mound;
        isRemoveButton = true;

        nameText.text = StringManager.Instance.GetText(removePlantKey);
        shortDescription.transform.parent.gameObject.SetActive(false);
        icon.sprite = removeIcon;
    }

    public void OnClick()
    {
        StartCoroutine(PlantSequence());
        /*if (isRemoveButton)
        {
            ownerMound.Shovel();
            return;
        }

        ownerMound.Plant(ownerPlant);*/
    }

    private IEnumerator PlantSequence()
    {
        if (this == null || gameObject == null) yield break;
        //The comment line is if wanted default sizing
        //var vfx = Instantiate(Resources.Load("ShrubsEffects"), ownerMound.transform.position, ownerMound.transform.rotation);
        GameObject vfx = Instantiate(Resources.Load("ShrubsEffects"), ownerMound.transform.position, ownerMound.transform.rotation) as GameObject;
        vfx.transform.localScale = Vector3.one * 3f;
        yield return new WaitForSeconds(.5f);
        Destroy(vfx);
        if (isRemoveButton)
        {
            ownerMound.Shovel();
            yield break;
            //return;
        }

        ownerMound.Plant(ownerPlant);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}

