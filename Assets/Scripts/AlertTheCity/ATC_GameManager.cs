using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager>
{
    public CameraMovement cameraMovement;
    public StructureManager structureManager;
    public ATC_InputManager inputManager;
    public ATC_RoadManager roadManager;
    public ACT_UIController uiController;
    public FireManager fireManager;
    private bool constructionMode;
    private bool assignMode;

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
        inputManager.OnMouseClick += structureManager.ClickStructre;
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
        inputManager.OnMouseClick += structureManager.PlaceHouse;
    }
    private void SpecialPlacementHandler()
    {

        ClearInputAction();
        inputManager.OnMouseClick += structureManager.PlaceSpecial;

    }
    private void HandleMouseClick(Vector3Int position)
    {
        Debug.Log(position);
       // roadManager.PlaceRoad(position);
    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
        cameraMovement.ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
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
}
