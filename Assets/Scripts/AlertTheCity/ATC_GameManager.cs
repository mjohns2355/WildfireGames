using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager>
{
    public CameraMovement cameraMovement;
    public ATC_RoadManager roadManager;
    public StructureManager structureManager;
    public ATC_InputManager inputManager;
    public ACT_UIController uiController;
    public FireManager fireManager;
    public bool canStartSim = false;
    public bool choseGoodOption = false;
    private bool constructionMode;
    private bool assignMode;
    private float timer = 0;
    private bool end = false;

    public ATC_dialogManager dialog;
    public GameObject evacNotice;


    public bool InAssignMode
    {
        get { return assignMode; }
    }


    public bool ConstructionMode
    {
        get { return constructionMode; }
    }

    private void Start()
    {
        //inputManager.OnMouseClick += structureManager.ClickStructre;
        //inputManager.OnMouseClick += HandleMouseClick;
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
    }

    private void RoadPlacementHandler()
    {

        ClearInputAction();
        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    private void HousePlacementHandler()
    {

        ClearInputAction();
        //inputManager.OnMouseClick += structureManager.PlaceHouse;
    }
    private void SpecialPlacementHandler()
    {

        ClearInputAction();
        //inputManager.OnMouseClick += structureManager.PlaceSpecial;

    }
    private void HandleMouseClick(Vector3Int position)
    {
        Debug.Log(position);
       // roadManager.PlaceRoad(position);
    }

    private void Update()
    {
        //Debug.Log(inputManager.cameraMovementVector);
        cameraMovement.MoveCamera(new Vector3(inputManager.cameraMovementVector.x, 0, inputManager.cameraMovementVector.y));
        //cameraMovement.ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
        cameraMovement.ZoomCamera(inputManager.cameraZoomAxis);

        if(canStartSim)
        {
            if(timer < 70)
            {
                timer += Time.deltaTime;
            }
            else if(!end)
            {
                end = true;
                fireManager.done = true;
                dialog.gameObject.SetActive(true);
                dialog.EndDialog();
            }
        }
    }

    private void ClearInputAction()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }

    public void ToggleConstructionMode()
    {
        constructionMode = !constructionMode;
        if (constructionMode == false)
        {
            ClearInputAction();
            inputManager.OnMouseClick += structureManager.ClickStructre;
        }
        uiController.UpdateConstructionMode(constructionMode);
        inputManager.OnConstructionMode(constructionMode);
        Debug.Log(constructionMode);
    }

    public void StartSimulation()
    {
        canStartSim = choseGoodOption;
        if (!choseGoodOption)
        {
            uiController.popUp.SetActive(true);
        }

        StartCoroutine(StartSimRoutine());
    }

    public void ToggleSimStatus(bool simStatus)
    {
        canStartSim = simStatus;
    }

    IEnumerator StartSimRoutine()
    {
        //Debug.Log("Start Coroutine");
        yield return new WaitUntil(()=>canStartSim);
        foreach (var menu in uiController.contextMenus)
        {
            menu.icon.gameObject.SetActive(false);
            menu.ApplyBehavior();
            if (!menu.gameObject.activeSelf) continue;
            menu.menu.SetActive(false);
            //if (!menu.gameObject.activeSelf) continue;
            //menu.gameObject.SetActive(false);

        }
        uiController.learnMorePanel.SetActive(false);
        StartCoroutine(fireManager.StartFireRoutine());
        WindZone.Instance.isStill = false;
        // update choice text
        StringBuilder sb = new StringBuilder();
        foreach(var pair in structureManager.GetPlayerChoicesDict())
        {
            sb.AppendLine(pair.Key + ": " + pair.Value.choiceName);
        }
        UpdateDebugText(sb.ToString());

        evacNotice.SetActive(true);

        // close all the menus and panels
        //uiController.OnSimulationStarted();
        yield return new WaitForSeconds(10f);
        Debug.Log($"Total cars sapwned {ATC_AIDirector.Instance.spawnedCarNum}");
    }
    public void ToggleAssignMode()
    {
        assignMode = !assignMode;
        if (assignMode == false)
        {
            ClearInputAction();
            inputManager.OnMouseClick += structureManager.ClickStructre;
        }
        uiController.UpdateConstructionMode(constructionMode);
        inputManager.OnConstructionMode(constructionMode);
        Debug.Log(assignMode);
    }

    void UpdateDebugText (string text)
    {
        //uiController.debugPanel.SetActive(true);
        //uiController.debugResultText.text = text;
        var dict = structureManager.GetPlayerChoicesDict();

        string twoCarRes = dict[HouseType.twoCar].endGameFeedback;
        string wuiRes = dict[HouseType.wui].endGameFeedback;
        string horseRes = dict[HouseType.horse].endGameFeedback;
        string kidsRes = dict[HouseType.kids].endGameFeedback;
        string petRes = dict[HouseType.pet].endGameFeedback;
        string elderRes = dict[HouseType.elderly].endGameFeedback;

        uiController.debugResultText.text = "The fire’s cause is not certain but likely from a downed powerline at the west edge of the town where our community meets the forest.\n\n";

        uiController.debugResultText.text += twoCarRes;

        uiController.debugResultText.text += "\n\nWildfire is always dangerous, but there are things we can all do to have a safer evacuation.\n\n";


        uiController.debugResultText.text += petRes + "\n\n";
        uiController.debugResultText.text += horseRes;


        uiController.debugResultText2.text = "We know some residents need more time and help getting out during an evacuation.\n\n";


        uiController.debugResultText2.text += elderRes + "\n\n";
        uiController.debugResultText2.text += kidsRes;


        uiController.debugResultText2.text += "\n\nHouses most at risk are the ones closest to the Wildland Urban Interface – the area where human development meets wild land and forest. \n\n";


        uiController.debugResultText2.text += wuiRes;


        uiController.debugResultText2.text += "\n\nOur community is grateful to the firefighters and emergency responders who made sure everyone got out alive. There is much to rebuild, and we will do it together. ";

        //uiController.debugPanel.SetActive(true);

    }
    public string[] ParseString( string str, char[] delimiterChars)
    {
        string[] words = str.Split(delimiterChars);

        return words;
    }

}
