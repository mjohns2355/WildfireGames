using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FYT_GameModeManager : MonoBehaviour
{
    
    [Header("Toggles")]
    public Toggle goBagToggle;
    public Toggle medicationToggle;
    public Toggle glassesToggle;
    public Toggle petToggle;

    [Header("Menus")]
    public GameObject toGoBagMenu;
    public GameObject toCritListMenu;
    public GameObject menu;
    public GameObject goBagGamePortrait;
    public GameObject critListGamePortrait;
    public GameObject goBagGameLandscape;
    public GameObject critListGameLandscape;

    [Header("Game Objects")]
    public GameObject medicationItem;
    public GameObject glassesItem;
    public GameObject medicationItemCrit;
    public GameObject glassesItemCrit;

    private bool goBag;
    private bool meds;
    private bool eyesAndEars;
    private bool pets;

    void Update()
    {
        if (menu.activeSelf)
        {
            goBag = goBagToggle.isOn;
            meds = medicationToggle.isOn;
            eyesAndEars = glassesToggle.isOn;
            pets = petToggle.isOn; 
            FYT_SettingsData.medsNeeded = meds;
            FYT_SettingsData.glassesNeeded = eyesAndEars;
            FYT_SettingsData.petNeeded = pets;
        }
    }

    public void continueGame()
    {
        toGoBagMenu.SetActive(goBag);
        toCritListMenu.SetActive(!goBag);
        menu.SetActive(false);
    }

    public void chooseGoBagLayout()
    {
        if (Screen.width < Screen.height)
        {
            // go to portrait mode
            toGoBagMenu.SetActive(false);
            goBagGamePortrait.SetActive(true);
        } else
        {
            Debug.Log("go to landscape");
            // go to landscape
        }
    }

    public void chooseCritListLayout()
    {
        if (Screen.width < Screen.height)
        {
            // go to portrait mode
            toCritListMenu.SetActive(false);
            critListGamePortrait.SetActive(true);
        } else
        {
            // go to landscape
        }
    }
}
