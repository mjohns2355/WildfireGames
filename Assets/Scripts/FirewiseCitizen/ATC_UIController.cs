using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class ATC_UIController : UnitySingleton<ATC_UIController>
{
    //public GameObject canvas;
    public GameObject popUp;
    public GameObject evacNotice;
    public ATC_dialogManager dialogManager;
    public ATC_HouseDialogManager houseDialogManager;
    public FC_StarScreen starScreen;
    //public GameObject toolsBar;
    public GameObject replayOverlay;
    public GameObject toolsBar;
    public ATC_StatsPanel statsPanel;
    public ATC_PauseMenu pauseMenu;
    //public GameObject endScreen;
    public GameObject learnMorePanel, restartPrompt;
    public TextMeshProUGUI levelText;
    public GameObject startPrompt, dialoguePanel;
    //public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement;
    public Button start, pause, learnMore, startAnyway, goBack, restartLevel, restartGame;
    public CanvasGroup loadingScreen;
    public float loadingTime;
    //public GameObject buildingMenu;
    public List<HouseStructure> selectedHouses = new List<HouseStructure> ();
    public List<StructureContextMenu> contextMenus = new List<StructureContextMenu>();
    public RectTransform windDirectionIndicator;
    public List<HouseIcon> icons = new List<HouseIcon>();
    Stack<GameObject> panelStack = new Stack<GameObject> ();
    private void Start()
    {
        //buildingMenu.SetActive(false);
        levelText.text = $"Level {GameManager.Instance.currentLevel + 1}";
        pause.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
            
        });
        start.onClick.AddListener(() =>
        {
            GameManager.Instance.StartSimulation();
            learnMore.interactable = false;
            start.interactable = false;
        });
        learnMore.onClick.AddListener(() =>
        {
            //learnMorePanel.SetActive(true);
           if(learnMorePanel.activeInHierarchy)   return;
           
           PushPanel(learnMorePanel);
        });

        startAnyway.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleSimStatus(true);
            popUp.SetActive(false);
            start.interactable = false;
            learnMore.interactable= false;

        });

        goBack.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleSimStatus(false);
            GameManager.Instance.StopCoroutine("StartSimRoutine");
            popUp.SetActive(false);
            start.interactable = true;
            learnMore.interactable = true;
        });
        HideLoadingScreen();
        //restartGame.onClick.AddListener(() =>
        //{
        //    GameManager.Instance.ResetGame(0);
        //});

        //restartLevel.onClick.AddListener(() =>
        //{
        //    GameManager.Instance.ResetGame();
        //});
        GameManager.Instance.SimStartsEvent.AddListener(OnSimStart);

        GameManager.Instance.SimEndsEvent.AddListener(OnSimEnd);
        //ShowDialog();
        //startPrompt.SetActive(true);


    }

    private void Update()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        var windDirection = GameManager.Instance.fireManager.wind.WindDirection;
        float angle = Vector3.SignedAngle(cameraForward, windDirection, Vector3.up);

        var targetRotation = Quaternion.Euler(0, 0, -angle);
        windDirectionIndicator.rotation = Quaternion.Lerp(windDirectionIndicator.rotation, targetRotation, Time.deltaTime * 5f);
    }
    void OnSimStart()
    {
        CloseAllUI();
        evacNotice.SetActive(true);
        statsPanel.gameObject.SetActive(true);
        start.interactable = false;
        learnMore.interactable = false;
        pause.interactable = true;
        windDirectionIndicator.gameObject.SetActive(true);
        toolsBar.SetActive(false);
    }
    void OnSimEnd()
    {
        statsPanel.ShowResultText();
        dialogManager.GenerateResult();
        replayOverlay.SetActive(false);
        if (GameManager.Instance.currentStage == LevelStage.AfterFirstSim)
        {
            ShowDialog();
        }
        else
        {
            dialogManager.ShowLocalNews();
        }
        windDirectionIndicator.gameObject.SetActive(false);
        toolsBar.SetActive(true);
        //ShowDialog();
    }

    void PrintStack()
    {
        Debug.Log("---- START ----");
        foreach(GameObject go in panelStack)
        {
            Debug.Log(go.name);
        }
        Debug.Log("---- END ----");
    }
    public void PushPanel(GameObject panel)
    {
        if(panelStack.Count > 0)
        {
            panelStack.Peek().SetActive(false);
        }

        panel.SetActive(true);
        panelStack.Push(panel);
        //PrintStack();
    }

    public void PopPanel()
    {
        GameObject topPanel = panelStack.Pop();
        topPanel.SetActive(false);

        if(panelStack.Count > 0)
        {
            panelStack.Peek().SetActive(true);
        }
        //PrintStack();
    }

    public void TogglePauseMenu(bool shouldShow)
    {
        if (shouldShow)
        {
            PushPanel(pauseMenu.gameObject);
            start.gameObject.SetActive(false);
        }
        else
        {
            PopPanel();
            start.gameObject.SetActive(true);
        }
    }


    public void ClearAllPanels()
    {
        while(panelStack.Count > 0)
        {
            GameObject panel = panelStack.Pop();
            panel.SetActive(false);
        }
    }

    public GameObject GetCurrentPanel()
    {
        return panelStack.Count > 0 ? panelStack.Peek() : null;
    }
    public void CloseAllUI()
    {
        foreach (var menu in contextMenus)
        {
            menu.icon.gameObject.SetActive(false);
            if (!menu.gameObject.activeSelf) continue;
            menu.menuUI.SetActive(false);
            //if (!menu.gameObject.activeSelf) continue;
            //menu.gameObject.SetActive(false);

        }
        
        evacNotice.SetActive(false);
        statsPanel.gameObject.SetActive(false);
        ClearAllPanels();
    }
    //public void UpdateConstructionMode(bool state)
    //{
    //    Text text = constructionButton.gameObject.GetComponentInChildren<Text>();
    //    if (state == true)
    //    {
    //        text.text = "Construction ON";
    //    }
    //    else
    //    {
    //        text.text = "Construction OFF";
    //    }
    //    //buildingMenu.SetActive(state);
    //}

    public void AddSelectedHouse(HouseStructure house)
    {
        selectedHouses.Add(house);
    }

    public void RemoveSelectedStructure(HouseStructure house)
    {
        selectedHouses.Remove(house);
    }

    public void AddMenu(StructureContextMenu menu)
    {
        if (!contextMenus.Contains(menu))
        {
            contextMenus.Add(menu);
        }
    }

    public StructureContextMenu FindMenu(HouseType type)
    {
        foreach(var menu in contextMenus)
        {
            var house = (HouseStructure)menu.owner;
            if (house.houseType == type)
            {
                return menu;
            }
        }
        return null;
    }
    
    public void ClampToWindow( RectTransform panelRectTransform, float offset)
    {
        Vector3[] corners = new Vector3[4];
        panelRectTransform.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        // Padding from screen edges
        Vector3 adjustedPosition = panelRectTransform.position;

        if (bottomLeft.x < offset)
        {
            adjustedPosition.x += offset - bottomLeft.x;
        }
        if (topRight.x > Screen.width - offset)
        {
            adjustedPosition.x -= topRight.x - (Screen.width - offset);
        }
        if (bottomLeft.y < offset)
        {
            adjustedPosition.y += offset - bottomLeft.y;
        }
        if (topRight.y > Screen.height - offset)
        {
            adjustedPosition.y -= topRight.y - (Screen.height - offset);
        }

        panelRectTransform.position = adjustedPosition;
    }


    public void ResetUI()
    {
        selectedHouses.Clear();
        contextMenus.Clear();
        var isFirstSim = GameManager.Instance.IsFirstSim;
        toolsBar.gameObject.SetActive(true);
        start.interactable = true;
        learnMore.interactable = !isFirstSim;
        pause.interactable = !isFirstSim;
        replayOverlay.SetActive(false);
        starScreen.gameObject.SetActive(false);
        icons.Clear();
        CloseAllUI();
        //if(GameManager.Instance.currentStage == LevelStage.BeforeFirstSim)
        //{
        //    levelText.text = "Instruction";
        //}
        //else
        //{
        //    levelText.text = $"Level 1";
        //}
        //levelText.text = $"Level {GameManager.Instance.CurrentLevel + 1}";
        //if (!dialogManager.isInstructionShown)
        //{
        //    ShowDialog();
        //}
        HideDialog();
        GameManager.Instance.SimStartsEvent.AddListener(OnSimStart);

        GameManager.Instance.SimEndsEvent.AddListener(OnSimEnd);
    }

    public void ShowDialog()
    {
        
        dialoguePanel.SetActive(true);
        //PushPanel(dialogManager.gameObject);
        //dialogManager.ShowDialogBox();
        toolsBar.SetActive(false);
    }

    public void HideDialog()
    {
        dialoguePanel.SetActive(false);
        //dialogManager.HideDialogBox();

    }

    public void ShowEndScreen()
    {
        //PushPanel(endScreen);
    }

    public void ToggleHouseIcons(bool state)
    {
        foreach(var icon in icons)
        {
            icon.gameObject.SetActive(state);
        }
    }
    //public void ShowEndDialog()
    //{
    //    PushPanel(dialogManager.gameObject);
    //    //dialogManager.EndDialog();
    //}

    public void ShowStarScreen()
    {
        starScreen.gameObject.SetActive(true);
        starScreen.ShowStars();
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.blocksRaycasts = true;
        loadingScreen.alpha = 1;
        GameManager.Instance.canControlCam = false;
        DOVirtual.DelayedCall(loadingTime, () =>
        {
            loadingScreen.DOFade(0, 0.5f).SetEase(Ease.InOutQuad).OnComplete(HideLoadingScreen);
        });
        
    }

    public void HideLoadingScreen()
    {
        loadingScreen.blocksRaycasts = false;
        GameManager.Instance.canControlCam = true;
        loadingScreen.alpha = 0;
    }
}
