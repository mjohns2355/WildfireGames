using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WarningPopupPanel : MonoBehaviour
{
    public Toggle p1Toggle, p2Toggle;
    public Button backButton, proceedButton;
    // Start is called before the first frame update
    void Start()
    {
        proceedButton.interactable = false;
        p1Toggle.isOn = false;
        p2Toggle.isOn = false;
        p1Toggle.onValueChanged.AddListener(CheckReadyStatus);
        p2Toggle.onValueChanged.AddListener(CheckReadyStatus);
        backButton.onClick.AddListener(ClosePopup);
        proceedButton.onClick.AddListener(ProceedAction);
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
        p1Toggle.isOn = false;
        p2Toggle.isOn = false;
        proceedButton.interactable = false;
    }

    private void CheckReadyStatus(bool arg0)
    {
        p1Toggle.targetGraphic.color = p1Toggle.isOn ? Color.green : Color.white;
        p2Toggle.targetGraphic.color = p2Toggle.isOn ? Color.green : Color.white;
        if (p1Toggle.isOn && p2Toggle.isOn)
        {
            proceedButton.interactable = true;
        }
        else
        {
            proceedButton.interactable = false;
        }
    }

    private void ProceedAction()
    {
        //Debug.Log("Both players are ready");
        HH_GameManager.Instance.EndRound();
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
