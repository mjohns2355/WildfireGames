using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_TutorialManager : MonoBehaviour
{
    public int totalSteps = 7;
    public int currentStep = 0;
    public List<string> tutorialText;
    HH_CameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        cameraController = HH_GameManager.Instance.cameraController;
        ShowStep(1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowStep(int step)
    {
        switch (step)
        {
            case 0:
                break;
            case 1:
                cameraController.Zoomcamera(HH_GameManager.Instance.h1CamPos, 80);
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
    }
}
