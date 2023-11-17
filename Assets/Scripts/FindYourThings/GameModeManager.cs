using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameModeManager : MonoBehaviour
{
    public TMP_Dropdown toGoBag; //next menu will be GoBag else critList
    public TMP_Dropdown addMedication; //Add medication to list yes or no
    public TMP_Dropdown addEyesAndEars; //Add glasses and hearing aids to list yes or no
    public TMP_Dropdown addPet; //Add Pet yes or no
    public bool goBag;
    public bool meds;
    public bool eyesAndEars;
    public bool pets;
    public GameObject toGoBagObject;
    public GameObject toCritListObject;
    public GameObject menu;
    public GameObject medicationItem;
    public GameObject glassesItem;
    public GameObject goBagGameUI;
    //public GameObject pets;
    private bool setItems;

    public void Start(){
    }
    void Update(){
        OnDropdownValueChanged();
    }
    public void OnDropdownValueChanged()
    {
        goBag = (toGoBag.value == 0);
        meds = (addMedication.value == 0);
        eyesAndEars = (addEyesAndEars.value == 0);
        pets = (addPet.value == 0);
        if(addMedication.value == 1 || addEyesAndEars.value == 1){
            if(goBagGameUI.activeSelf){
                medicationItem.SetActive(meds);
                glassesItem.SetActive(eyesAndEars);
                addMedication.value = 0;
                addEyesAndEars.value = 0;
            }
        }
    }
    public void continueGame(){
        toGoBagObject.SetActive(goBag);
        toCritListObject.SetActive(!goBag);
        menu.SetActive(false);
    }
}
