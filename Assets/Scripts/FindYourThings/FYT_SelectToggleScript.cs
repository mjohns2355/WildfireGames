using UnityEngine;
using UnityEngine.UI;

public class FYT_SelectToggleScript : MonoBehaviour
{
    [Header("Q1")]
    public Toggle goBagToggleYes;
    public Toggle goBagToggleNo;

    [Header("Q2")]
    public Toggle medicationToggleYes;
    public Toggle medicationToggleNo;

    [Header("Q3")]
    public Toggle eyesAndEarsToggleYes;
    public Toggle eyesAndEarsToggleNo;

    [Header("Q4")]
    public Toggle petToggleYes;
    public Toggle petToggleNo;

    void Start()
    {
        SetupToggleEvents(goBagToggleYes, goBagToggleNo);
        SetupToggleEvents(medicationToggleYes, medicationToggleNo);
        SetupToggleEvents(eyesAndEarsToggleYes, eyesAndEarsToggleNo);
        SetupToggleEvents(petToggleYes, petToggleNo);
        
        goBagToggleYes.isOn = true;
        medicationToggleYes.isOn = true;
        eyesAndEarsToggleYes.isOn = true;
        petToggleYes.isOn = true;
    }

    void SetupToggleEvents(Toggle yesToggle, Toggle noToggle)
    {
        yesToggle.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                noToggle.isOn = !value;
            }
            else if (!noToggle.isOn)
            {
                noToggle.isOn = true;
            }
        });

        noToggle.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                yesToggle.isOn = !value;
            }
            else if (!yesToggle.isOn)
            {
                yesToggle.isOn = true;
            }
        });
    }
}
