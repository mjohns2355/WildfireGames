using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutscene : MonoBehaviour
{
    //dialog strings to be set in the inspector
    public string[] dialogs;
    public string[] dialogs2;
    public string[] dialogs3;
    public string[] dialogs4;
    public string[] dialogs5;

    private int counter = 0; //counter for text within current cutscene
    private int cutSceneCount = 0; // countr for current cutscene

    // reference to the main menu script
    public mainMenu mainScript;

    // references to the objects for the characters in the cutscene
    public GameObject p1;
    public GameObject p2;

    // text mesh pro objects for the text for each character
    public TMPro.TextMeshProUGUI p1Text;
    public TMPro.TextMeshProUGUI p2Text;

    private void OnEnable()
    {
        // if we are in hosted mode, no cut scene, proceed to the main menu
        if (mainScript.hostedMode)
        {
            mainScript.ExitToMain();
        } else
        {
            // if we are in story mode play the next cutscene
            NextStep();
        }
    }

    // called from scene, plays the first cutscene
    public void GoTime()
    {
        counter = dialogs.Length;
        NextStep();
    }

    public void NextStep()
    {
        // if we have finished the dialogs for this cutscene, progress to next cut scene and disable object
        if(counter >= dialogs.Length)
        {
            mainScript.NextGame();
            counter = 0;
            p1.SetActive(true);
            p2.SetActive(false);
            cutSceneCount++;
            switch(cutSceneCount){
                case 1:
                    dialogs = dialogs2;
                    break;
                case 2: 
                    dialogs = dialogs3;
                    break;
                case 3:
                    dialogs = dialogs4;
                    break;
                case 4:
                    dialogs = dialogs5;
                    break;
                default:
                    break;
            }
            gameObject.SetActive(false);
        }
        // if we are mid cutscene, display p1 or p2 with corresponding text
        else if(counter %2 == 0)
        {
            p1Text.text = dialogs[counter];
            p1.SetActive(true);
            p2.SetActive(false);

            counter++;
        } else
        {
            p2Text.text = dialogs[counter];
            p2.SetActive(true);
            p1.SetActive(false);

            counter++;
        }
    }
}
