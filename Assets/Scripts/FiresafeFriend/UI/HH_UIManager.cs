using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HH_UIManager : MonoBehaviour
{
    public StorePanel storePanel;
    public PurchasePopup purchasePopup;
    public Button leftArrow, rightArrow, earnMoreMoney, startFireBtn, endRoundBtn, repairBtn,moveBtn;
    public InventoryUI inventoryPanel;
    public Transform floatingIcons;
    public FF_EndScreensManager endScreenManager;
    public FF_QuizPopupUI quizPopup;
    public WarningPopupPanel warningPopup;
    public FF_PlantsMenu plantsMenu;
    public GameObject bubblePrefab,startText, modeToggle;
    public GameObject pauseMenu,aftermathPopup,joinConcilPopup,competitionAnnouncement,competitionBanner,competitionNotice,fireAnnouncement;
    public TextMeshProUGUI aftermathPlayerText, repairDescText,joinConcilReward;
    public Action OnCompetitionResultEnabled;
    [Header("Banner Timings")]
    public float stretchDuration = 0.5f;  
    public float displayDuration = 1f;    
    public float fadeDuration = 0.5f;

    private bool firstCompetitionAnnouncement = true;
    private bool canShow = false;
    public Toggle languageToggle;

    // Start is called before the first frame update
    void Start()
    {
        
        leftArrow.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.SwitchPlayer("P1");


        });
        rightArrow.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.SwitchPlayer("P2");
        });
        languageToggle.onValueChanged.AddListener((bool isOn) => {
            DOVirtual.DelayedCall(0.01f, () => RefreshAllUI());
        });
    

        HH_GameManager.Instance.OnRoundStart += OnRoundStart;
        HH_GameManager.Instance.OnRoundEnd += OnRoundEnd;

        Toggle toggle = modeToggle.GetComponent<Toggle>();

        // Synchronize Toggle state with GameManager on startup
        toggle.isOn = HH_GameManager.Instance.IsPlantMode;

        // 1. Subscribe to GameManager's OnPlantModeChanged event
        HH_GameManager.Instance.OnPlantModeChanged += (mode) =>
        {

            // Only update the Toggle if the value is different
            if (toggle.isOn != mode)
            {
                // Temporarily remove the listener to avoid circular event
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

                // Update the Toggle state
                toggle.isOn = mode;

                // Re-attach the listener
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        };

        // 2. Add listener to handle user input
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        earnMoreMoney.onClick.AddListener(ShowQuizPopup);

        if (HH_GameManager.Instance.isTutorial) return;

        repairBtn.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.RepairHouse();
            aftermathPopup.SetActive(false);
        });

        moveBtn.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.MoveHouse();
            aftermathPopup.SetActive(false);
        });

        //toggle.interactable = false;
    }

    public void SetModeToggleState(bool state)
    {
        modeToggle.GetComponent<Toggle>().interactable = state;
    }

    private void OnToggleValueChanged(bool value)
    {
        HH_GameManager.Instance.ChangeGameMode(value);
    }

    public void OnCloseCompetitionNotice()
    {
        firstCompetitionAnnouncement = false;
        competitionNotice.SetActive(false);
        competitionAnnouncement.SetActive(false);
        canShow = true;
    }
    public void TogglePauseMenu(bool state)
    {
        pauseMenu.SetActive(state);
    }
    public void ShowWarningPopup()
    {
        warningPopup.gameObject.SetActive(true);
    }

    public void ShowQuizPopup()
    {
        quizPopup.gameObject.SetActive(true);
        quizPopup.InitQuizPopup();
    }
    public void ShowAftermathScreen()
    {
        var currentPlayer = HH_GameManager.Instance.currentPlayer;
        aftermathPlayerText.text = StringManager.Instance != null ? StringManager.Instance.GetText(currentPlayer.playerTag == "P1" ? "player1Text" : "player2Text") : (currentPlayer.playerTag == "P1" ? "Player 1" : "Player 2");
        //aftermathPlayerText.text = currentPlayer.playerTag == "P1" ? "Player 1" : "Player 2";
        var repairCost = currentPlayer.GetRepairCost();
        repairDescText.text = $"You will have to pay ${repairCost} to fix your house";
        aftermathPopup.SetActive(true);
       
    }

    public void ShowStoreScreen(HousePartType partType, bool isPublic = false/*, PurchaseFloatingButton clickedButton = null*/)
    {
        if (isPublic)
        {
            Debug.Log("Show store of public fences");
        }
        storePanel.gameObject.SetActive(true);
        //storePanel.SetCurrentPurchaseFloatingButton(clickedButton);
        storePanel.ShowStorePanel(partType, isPublic);
    }

    public void ToggleJoinConcilPopup(bool state,int reward)
    {
        joinConcilPopup.SetActive(state);
        joinConcilReward.text = $"Earn ${reward}";
    }
    public void ShowPlantsMenu(FF_DirtMound owner)
    {
        //if (plantsMenu.gameObject.activeSelf) HidePlantsMenu();
        plantsMenu.gameObject.SetActive(true);
        plantsMenu.ShowPlantsMenu(owner);
    }

    public void HidePlantsMenu()
    {
        plantsMenu.ClosePlantsMenu();
    }
    public void HideStoreScreen()
    {
        storePanel.HideStorePanel();
    }

    public void ShowPurchasePopup(HousePartInfo partInfo = null, bool isWarning = false, bool isPlant = false)
    {
        //Debug.Log($"Show PUBLIC purchase popup: {partInfo.isPublic}");
        purchasePopup.gameObject.SetActive(true);
        if (isWarning)
        {
            purchasePopup.ShowWarningScreen();
            return;
        }
        if(partInfo != null)
        {
            purchasePopup.InitPurchasePopup(partInfo);
            purchasePopup.ShowPurchaseScreen();
        }

        if (isPlant)
        {
            purchasePopup.ShowRemoveScreen();
        }
    }
    public void HidePurchasePopup(HousePartInfo partInfo, bool shouldShowStore = true)
    {
        purchasePopup.gameObject.SetActive(false);
        //HH_GameManager.Instance.currentPlayer.ToggleAllPurchaseIcons(true);

        if (partInfo && shouldShowStore)
        {
            storePanel.gameObject.SetActive(true);
            storePanel.UpdateStorePanel();
            //storePanel.ShowStorePanel(partInfo.housePartType, partInfo.isPublic);
        }


    }

    public void ToggleInventory(bool state)
    {
        inventoryPanel.gameObject.SetActive(state);

        if (!inventoryPanel.inventoryUI.activeInHierarchy) return;
        // make sure the inventory grid is disabled when switching player
        inventoryPanel.inventoryUI.SetActive(false);
    }

    
    public void OnRoundStart()
    {
        ToggleInventory(true);
        endRoundBtn.gameObject.SetActive(true);
        startFireBtn.gameObject.SetActive(false);
        startText.SetActive(false);
        modeToggle.SetActive(true);
        canShow = false;    
    }
    public void OnRoundEnd()
    {
        ToggleInventory(false);
        HideStoreScreen();
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
        modeToggle.SetActive(false);
        endRoundBtn.gameObject.SetActive(false);
        ToggleEarnMoreMoneyButton(false);
        HidePlantsMenu();
    }

    public PurchaseFloatingButton SpawnBubble()
    {
        return Instantiate(bubblePrefab, floatingIcons).GetComponent<PurchaseFloatingButton>();
    }

    public void ToggleEarnMoreMoneyButton(bool state)
    {
        if (state)
        {
            earnMoreMoney.gameObject.SetActive(true);
            var rect = earnMoreMoney.GetComponent<RectTransform>();
            rect.DOScale(Vector3.one * 1.5f, 0.3f)
                         .SetLoops(4, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
        }
        else
        {
            earnMoreMoney.gameObject.SetActive(false);
        }
    }

    public void ShowEndScreen(bool isFire, float p1Score, float p2Score, float p1Stars, float p2Stars)
    {
        if (isFire)
        {
            endScreenManager.gameObject.SetActive(true);
            endScreenManager.ShowFireResultScreen(p1Score, p2Score, p1Stars, p2Stars);
        }
        else
        {
            ShowCompetitionAnnouncement();
            StartCoroutine(ShowCompetitionResult(p1Score, p2Score));
        }
    }
    
    IEnumerator ShowCompetitionResult(float p1Score, float p2Score)
    {
        yield return new WaitUntil(() => canShow);
        endScreenManager.gameObject.SetActive(true);
        endScreenManager.ShowCompetitionResult((int)p1Score, (int)p2Score);
        OnCompetitionResultEnabled.Invoke();
    }

    public void ShowFireAnnouncement()
    {
        fireAnnouncement.SetActive(true);
        var rect = fireAnnouncement.GetComponentInChildren<RectTransform>();
        var canvasGroup = fireAnnouncement.GetComponent<CanvasGroup>();
        rect.localScale = new Vector3(0f, 1f, 1f);
        canvasGroup.alpha = 1f;
        var seq = StrechAndFadeEffect(rect, canvasGroup);
        seq.OnComplete(() => canvasGroup.gameObject.SetActive(true));
    }

    public void ShowCompetitionAnnouncement()
    {
        competitionAnnouncement.SetActive(true);
        var rect = competitionBanner.GetComponent<RectTransform>();
        var canvasGroup = competitionBanner.GetComponent<CanvasGroup>();
        rect.localScale = new Vector3(0f, 1f, 1f);
        canvasGroup.alpha = 1f;
        var seq = StrechAndFadeEffect(rect, canvasGroup);
        seq.OnComplete(() =>
        {
            if (firstCompetitionAnnouncement)
            {
                competitionNotice.SetActive(true);
            }
            else
            {
                competitionNotice.SetActive(false);
                competitionAnnouncement.SetActive(false);
                canShow = true;
            }
            
        });
    }

    public Sequence StrechAndFadeEffect(RectTransform rect, CanvasGroup canvasGroup)
    {
        Sequence seq = DOTween.Sequence();

        // 1) Stretch X from 0 → 1 with a nice “pop” ease
        seq.Append(
            rect
                .DOScaleX(1f, stretchDuration)
                .SetEase(Ease.OutBack)
        );

        // 2) Hold full size for displayDuration
        seq.AppendInterval(displayDuration);

        // 3) Fade the whole CanvasGroup out
        seq.Append(
            canvasGroup
                .DOFade(0f, fadeDuration)
                .SetEase(Ease.InQuad)
        );

        return seq;
    }

    public void RefreshAllUI()
    {
        if (storePanel.gameObject.activeInHierarchy)
        {
            storePanel.RefreshLanguage();
        }

        if (inventoryPanel.gameObject.activeInHierarchy)
        {
            inventoryPanel.RefreshLanguage();
        }

        if (plantsMenu != null && plantsMenu.gameObject.activeInHierarchy)
        {
            plantsMenu.RefreshLanguage();
        }
    }
}
