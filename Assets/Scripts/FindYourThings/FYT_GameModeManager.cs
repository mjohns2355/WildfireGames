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
            if (FYT_SettingsData.petNeeded == false)
            {
                goBagGamePortrait.GetComponent<FYT_PetManager>().enabled = false;
            }
        } else
        {
            // go to landscape
            toGoBagMenu.SetActive(false);
            goBagGameLandscape.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                goBagGameLandscape.GetComponent<FYT_PetManager>().enabled = false;
            }
        }
    }

    public void chooseCritListLayout()
    {
        if (Screen.width < Screen.height)
        {
            // go to portrait mode
            toCritListMenu.SetActive(false);
            critListGamePortrait.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                critListGamePortrait.GetComponent<FYT_PetManager>().enabled = false;
            }
        } else
        {
            // go to landscape
            toCritListMenu.SetActive(false);
            critListGameLandscape.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                critListGameLandscape.GetComponent<FYT_PetManager>().enabled = false;
            }
        }
    }
}
