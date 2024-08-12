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
    public bool hasChoseGoodOption = false;
    private bool constructionMode;
    private bool assignMode;
    private float timer = 0;
    private bool end = false;

    public ATC_dialogManager dialog;


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
        canStartSim = hasChoseGoodOption;
        if (!hasChoseGoodOption)
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
        GameObject[] icons = GameObject.FindGameObjectsWithTag("typeIcon");
        foreach (GameObject g in icons)
        {
            g.SetActive(false);
        }
        foreach (var menu in uiController.contextMenus)
        {
            menu.ApplyBehavior();
        }
        StartCoroutine(fireManager.StartFireRoutine());
        WindZone.Instance.isStill = false;

        StringBuilder sb = new StringBuilder();
        foreach(var pair in structureManager.GetPlayerChoicesDict())
        {
            sb.AppendLine(pair.Key + ": " + pair.Value.choiceName /*+ "\n"*/);
        }
        UpdateDebugText(sb.ToString());
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
        uiController.debugPanel.SetActive(true);
        uiController.debugResultText.text = text;
        
    }
    public string[] ParseString( string str, char[] delimiterChars)
    {
        string[] words = str.Split(delimiterChars);

        return words;
    }
}
