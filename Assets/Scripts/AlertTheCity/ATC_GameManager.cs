using cakeslice;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : UnitySingleton<GameManager>
{
    public CameraMovement cameraMovement;
    public ATC_RoadManager roadManager;
    public StructureManager structureManager;
    public ATC_InputManager inputManager;
    public FireManager fireManager;
    //public ATC_dialogManager dialogManager;
    public bool canStartSim = false;
    public bool choseGoodOption = false;
    private float timer = 0;
    private bool end = false;
    public bool FirstTimeLoading { get; private set; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FirstTimeLoading = false;
        ATC_UIController.Instance.ShowStartScreen();
        //inputManager.OnMouseClick += structureManager.ClickStructre;
        //inputManager.OnMouseClick += HandleMouseClick;
        //uiController.OnRoadPlacement += RoadPlacementHandler;
        //uiController.OnHousePlacement += HousePlacementHandler;
        //uiController.OnSpecialPlacement += SpecialPlacementHandler;
    }

    //private void RoadPlacementHandler()
    //{
    //    ClearInputAction();
    //    inputManager.OnMouseClick += roadManager.PlaceRoad;
    //    inputManager.OnMouseHold += roadManager.PlaceRoad;
    //    inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    //}

    //private void HousePlacementHandler()
    //{

    //    ClearInputAction();
    //    //inputManager.OnMouseClick += structureManager.PlaceHouse;
    //}
    //private void SpecialPlacementHandler()
    //{

    //    ClearInputAction();
    //    //inputManager.OnMouseClick += structureManager.PlaceSpecial;

    //}
    //private void HandleMouseClick(Vector3Int position)
    //{
    //    Debug.Log(position);
    //   // roadManager.PlaceRoad(position);
    //}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) { NextLevel(); }
        cameraMovement.MoveCamera(new Vector3(inputManager.cameraMovementVector.x, 0, inputManager.cameraMovementVector.y));
        cameraMovement.ZoomCamera(inputManager.cameraZoomAxis);

        if(canStartSim)
        {
            // don't forget to set it back to 70
            if(timer < 5)
            {
                timer += Time.deltaTime;
            }
            else if(!end)
            {
                end = true;
                fireManager.done = true;
                ATC_UIController.Instance.ShowEndDialog();
                //ToggleGamePause(true);
                //dialogManager.gameObject.SetActive(true);

                //dialogManager.EndDialog();
            }
        }
    }


    //private void ClearInputAction()
    //{
    //    inputManager.OnMouseClick = null;
    //    inputManager.OnMouseHold = null;
    //    inputManager.OnMouseUp = null;
    //}

    //public void ToggleConstructionMode()
    //{
    //    constructionMode = !constructionMode;
    //    if (constructionMode == false)
    //    {
    //        ClearInputAction();
    //        inputManager.OnMouseClick += structureManager.ClickStructre;
    //    }
    //    //uiController.UpdateConstructionMode(constructionMode);
    //    inputManager.OnConstructionMode(constructionMode);
    //    Debug.Log(constructionMode);
    //}

    public void StartSimulation()
    {
        if(!FirstTimeLoading)
        {
            canStartSim = choseGoodOption;
            if (!choseGoodOption)
            {
                ATC_UIController.Instance.popUp.SetActive(true);
            }
        }
        else
        {
            canStartSim = true;
        }
        StartCoroutine(StartSimRoutine());
    }

    public void ToggleSimStatus(bool simStatus)
    {
        canStartSim = simStatus;
    }

    IEnumerator StartSimRoutine()
    {
        yield return new WaitUntil(()=>canStartSim);
        FirstTimeLoading = false;
        // close all the menus and panels
        ATC_UIController.Instance.CloseAllUI();
        foreach(var menu in ATC_UIController.Instance.contextMenus)
        {
            menu.ApplyBehavior();
        }
        StartCoroutine(fireManager.StartFireRoutine());
        fireManager.wind.isStill = false;

        // update choice text
        ATC_UIController.Instance.GenerateGameEndSummary(structureManager.GetPlayerChoicesDict());
        ATC_UIController.Instance.evacNotice.SetActive(true) ;

        //debug
        yield return new WaitForSeconds(10f);
        Debug.Log($"Total cars sapwned {ATC_AIDirector.Instance.spawnedCarNum}");
    }

    public void ToggleGamePause(bool state)
    {
        Time.timeScale = state ? 0 : 1;
    }
    //public void ToggleAssignMode()
    //{
    //    assignMode = !assignMode;
    //    if (assignMode == false)
    //    {
    //        ClearInputAction();
    //        inputManager.OnMouseClick += structureManager.ClickStructre;
    //    }
    //    uiController.UpdateConstructionMode(constructionMode);
    //    inputManager.OnConstructionMode(constructionMode);
    //    Debug.Log(assignMode);
    //}


    //public string[] ParseString( string str, char[] delimiterChars)
    //{
    //    string[] words = str.Split(delimiterChars);

    //    return words;
    //}

    public void ResetGame()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FirstTimeLoading = true;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        timer = 0;
        ATC_UIController.Instance.ResetUI();
        structureManager = FindObjectOfType<StructureManager>();
        roadManager = FindObjectOfType<ATC_RoadManager>();
        inputManager = FindObjectOfType<ATC_InputManager>();
        fireManager = FindObjectOfType<FireManager>();
        cameraMovement = FindObjectOfType<CameraMovement>();
        //dialogManager = FindObjectOfType<ATC_dialogManager>();
        //uiController = FindObjectOfType<ATC_UIController>();
    }
}
