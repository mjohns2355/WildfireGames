using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameModeManager : MonoBehaviour
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
            if((medicationToggle.isOn) == false){
                Destroy(medicationItem);
            }
            if((eyesAndEarsToggle.isOn) == false){
                Destroy(glassesItem);
            }
            else if((medicationToggle.isOn) == false && (eyesAndEarsToggle.isOn) == false){
                Destroy(medicationItem);
                Destroy(glassesItem);
            }
        }
        else if(CritListGameUI.activeSelf){
            Debug.Log("critListUI active");
            if((medicationToggle.isOn) == false){
                Destroy(medicationItemCrit);
                Debug.Log("1");
            }
            if((eyesAndEarsToggle.isOn) == false){
                Destroy(glassesItemCrit);
                Debug.Log("2");
            }
            else if((medicationToggle.isOn) == false && (eyesAndEarsToggle.isOn) == false){
                Destroy(medicationItemCrit);
                Destroy(glassesItemCrit);
                Debug.Log("3");
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
