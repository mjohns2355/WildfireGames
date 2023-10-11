using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutscene : MonoBehaviour
{
    public string[] dialogs;
    public string[] dialogs2;
    public string[] dialogs3;
    public string[] dialogs4;
    public string[] dialogs5;
    private int counter = 0;
    private int cutSceneCount = 0;
    public mainMenu mainScript;

    public GameObject p1;
    public GameObject p2;
    public TMPro.TextMeshProUGUI p1Text;
    public TMPro.TextMeshProUGUI p2Text;

    // Start is called before the first frame update
    void Start()
    {
       // NextStep();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        if (mainScript.hostedMode)
        {
            mainScript.ExitToMain();
        } else
        {

            NextStep();
        }
    }

    public void GoTime()
    {
        counter = dialogs.Length;
        NextStep();
    }

    public void NextStep()
    {
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
