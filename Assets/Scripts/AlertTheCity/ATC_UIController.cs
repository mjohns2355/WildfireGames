using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ATC_UIController : UnitySingleton<ATC_UIController>
{
    public GameObject canvas;
    public GameObject popUp;
    public GameObject evacNotice;
    public ATC_dialogManager dialogManager;
    public TextMeshProUGUI debugResultText;
    public TextMeshProUGUI debugResultText2;
    public ATC_StatsPanel statsPanel;
    public ATC_PauseMenu pauseMenu;
    //public GameObject debugPanel;
    public GameObject learnMorePanel;
    //public HouseInfo currentHouseInfo;
    //public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement;
    public Button start, pause, learnMore, startAnyway, goBack;
    //public GameObject buildingMenu;
    public List<HouseStructure> selectedHouses = new List<HouseStructure> ();
    //public ShelterStructure selectedShelter;
    //public List<Sprite> iconList;
    public List<StructureContextMenu> contextMenus = new List<StructureContextMenu>();

    Stack<GameObject> panelStack = new Stack<GameObject> ();
    private void Start()
    {
        //buildingMenu.SetActive(false);
       
        pause.onClick.AddListener(() =>
        {
            PushPanel(pauseMenu.gameObject);
            
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
           PushPanel(learnMorePanel);
        });

        startAnyway.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleSimStatus(true);
            popUp.SetActive(false);
            //PushPanel(popUp);
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

        GameManager.Instance.SimStartsEvent.AddListener(() =>
        {
            CloseAllUI();
            evacNotice.SetActive(true);
            statsPanel.gameObject.SetActive(true);

        });

        GameManager.Instance.SimEndsEvent.AddListener(() =>
        {
            statsPanel.ShowResultText();
            ShowEndDialog();
        });
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
        PrintStack();
    }

    public void PopPanel()
    {
        GameObject topPanel = panelStack.Pop();
        topPanel.SetActive(false);

        if(panelStack.Count > 0)
        {
            panelStack.Peek().SetActive(true);
        }
        PrintStack();
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
            if (house.HouseType == type)
            {
                return menu;
            }
        }
        return null;
    }
    public void GenerateGameEndSummary(Dictionary<HouseType,HouseChoice> playerChoicesDict)
    {
        var currentLevel = GameManager.Instance.CurrentLevel;
        var dict = playerChoicesDict;

        string twoCarRes = dict[HouseType.twoCar].endGameFeedback;
        string wuiRes = dict[HouseType.wui].endGameFeedback;

        debugResultText.text = "The fire’s cause is not certain but likely from a downed powerline at the west edge of the town where our community meets the forest.\n\n";

        debugResultText.text += twoCarRes + "\n\n";

        if(currentLevel != 0)
        {
            debugResultText.text += "Wildfire is always dangerous, but there are things we can all do to have a safer evacuation.\n\n";

            string petRes = dict[HouseType.pet].endGameFeedback;
            string horseRes = dict[HouseType.horse].endGameFeedback;
            debugResultText.text += petRes + "\n\n";
            debugResultText.text += horseRes + "\n\n";
        }


        if (currentLevel != 0)
        {
            debugResultText2.text = "We know some residents need more time and help getting out during an evacuation.\n\n";

            string kidsRes = dict[HouseType.kids].endGameFeedback;
            string elderRes = dict[HouseType.elderly].endGameFeedback;
            debugResultText2.text += elderRes + "\n\n";
            debugResultText2.text += kidsRes + "\n\n";
        }
 


        debugResultText2.text += "Houses most at risk are the ones closest to the Wildland Urban Interface – the area where human development meets wild land and forest. \n\n";


        debugResultText2.text += wuiRes + "\n\n";


        debugResultText2.text += "Our community is grateful to the firefighters and emergency responders who made sure everyone got out alive. There is much to rebuild, and we will do it together. ";

        //uiController.debugPanel.SetActive(true);


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
        CloseAllUI();
    }

    public void ShowStartScreen()
    {
        PushPanel(dialogManager.gameObject);
    }
    public void ShowEndDialog()
    {
        PushPanel(dialogManager.gameObject);
        dialogManager.EndDialog();
    }
}
