using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SD_TVStuff : MonoBehaviour
{
    [SerializeField] private GameObject TVUI;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private int count;
    private List<string> texts = new List<string>();

    void Start()
    {

        string longText1 = "The AQI monitor alerts you that the air quality is unsafe! Endeavor to make " +
                           "changes and discover what will bring it back to normal levels.";
        string longText2 = "Attention, intrepid resident! The air outside is starting to resemble a dragon's breath" +
                            " after a fiery feast. Try to build an airfilter with what you have to transform your fortress!";

        string longText3 = "Remember, the only haze we want indoors is the mystery around why socks disappear in the "+
                            "laundry. Let's make your home a smoke-free haven, because even our air deserves a spa day!";
        texts.Add(longText1);
        texts.Add(longText2);
        texts.Add(longText3);

        textMeshPro.text = texts[0];
    }

    // Update is called once per frame
    public void RemoveTVPOPUP()
    {
        TVUI.SetActive(false);
        SD_GameSateManager.Instance.switchGameState(SD_GameState.Ongoing);
        count++;
        if(count < texts.Count)
        {
            textMeshPro.text = texts[count];
        }
        GameObject checkObject = GameObject.Find("MANAGER");
        SD_UITest startTVTimer = checkObject.GetComponent<SD_UITest>();
        startTVTimer.startTVTimer();
        startTVTimer.TVcounter();

    }
}
