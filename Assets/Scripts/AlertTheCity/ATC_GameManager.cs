using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public StructureManager structureManager;
    public ATC_InputManager inputManager;
    public ATC_RoadManager roadManager;
    public ACT_UIController uiController;
    private void Start()
    {
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
    }

    private void ClearInputAction()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }
}
