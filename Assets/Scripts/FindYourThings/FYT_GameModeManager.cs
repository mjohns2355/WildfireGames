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
    public Toggle eyesAndEarsToggle;
    public Toggle petToggle;

    [Header("Menus")]
    public GameObject toGoBagObject;
    public GameObject toCritListObject;
    public GameObject menu;
    public GameObject goBagGameUI;
    public GameObject CritListGameUI;

    [Header("Game Objects")]
    public GameObject medicationItem;
    public GameObject glassesItem;
    public GameObject medicationItemCrit;
    public GameObject glassesItemCrit;

    private bool goBag;
    private bool meds;
    private bool eyesAndEars;
    private bool pets;

    void Update(){
        goBag = goBagToggle.isOn;
        meds = medicationToggle.isOn;
        eyesAndEars = eyesAndEarsToggle.isOn;
        pets = petToggle.isOn;

        if(goBagGameUI.activeSelf){
            FYT_SettingsData.medsNeeded = meds;
            FYT_SettingsData.glassesNeeded = eyesAndEars;
            if(pets == false){
                //Debug.Log("No pets.");
            }
        }
        if(CritListGameUI.activeSelf){
            FYT_SettingsData.medsNeeded = meds;
            FYT_SettingsData.glassesNeeded = eyesAndEars;
            if(pets == false){
                //Debug.Log("No pets.");
            }
        }
    }

    public void continueGame()
    {
        toGoBagObject.SetActive(goBag);
        toCritListObject.SetActive(!goBag);
        menu.SetActive(false);
    }
}
