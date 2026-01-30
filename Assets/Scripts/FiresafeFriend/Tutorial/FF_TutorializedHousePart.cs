using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FF_TutorializedHousePart : FF_TutorializedObject
{
    public HousePartType partType;
    public Button storePurchaseButton;
    public HighlightMesh highlightMesh;
    public GameObject plantModeToggle;
    public Transform camH1;

    PurchaseFloatingButton bubble;
    HouseManager houseManager;
    List<BaseHousePartObject> partObjects;
    InventoryUI inventory;
    InventoryItem oldItem, newItem;

    public string inventoryIntroKey = "tutText11";
    public string inventoryReplaceKey = "tutText12";
    public string inventoryCompleteKey = "tutText14";
    public string inventoryRetryKey = "tutText13";
    public string toggleHouseModeKey = "tutText9";
    public string upgradeRoofIntroKey = "tutText10";
    public string purchaseMaterialKey = "tutText9";

    public override void OnTutorialStepStart()
    {
        inventory = HH_GameManager.Instance.uiManager.inventoryPanel;

        plantModeToggle.GetComponent<Toggle>().onValueChanged.AddListener((vlaue) =>
        {
            SwitchToHouseMode();
        });

        storePurchaseButton.onClick.AddListener(() =>
        {
            canClick = true;

            foreach (var part in partObjects)
            {
                part.isClickable = false;
            }

            HH_GameManager.Instance.inputManager.OnObjectSelected -= OnPartTapped;
            bubble.gameObject.SetActive(false);
            ShowInventoryButton();
        });

        onClick.AddListener(ShowToggle);
    }

    public void ShowInventoryButton()
    {
        FF_TutorialManager.Instance.tutorialText.text =
            StringManager.Instance.GetText(inventoryIntroKey);

        inventory.gameObject.SetActive(true);

        var inventoryBtn = inventory.inventoryButton;
        inventoryBtn.onClick.AddListener(OnInventoryOpened);
        inventoryBtn.gameObject.SetActive(true);
        inventoryBtn.interactable = false;

        Sequence sq = DOTween.Sequence();
        sq.Append(ScaleEffect(inventoryBtn.GetComponent<RectTransform>()));
        sq.onComplete += () =>
        {
            inventoryBtn.interactable = true;
        };
    }

    void OnInventoryOpened()
    {
        StartCoroutine(WaitAndInitializeInventory());
    }

    IEnumerator WaitAndInitializeInventory()
    {
        while (StringManager.Instance == null || !StringManager.Instance.IsReady)
        {
            yield return null;
        }

        FF_TutorialManager.Instance.tutorialText.text = StringManager.Instance.GetText(inventoryReplaceKey);

        CategoryButton[] allCategoryButtons = inventory.GetComponentsInChildren<CategoryButton>(true);
        foreach (var btn in allCategoryButtons)
        {
            if (btn.category == HousePartType.Roof)
            {
                btn.categoryText.text = StringManager.Instance.GetText("Roof");
            }
        }

        oldItem = inventory.items[0];
        newItem = inventory.items[1];

        newItem.button.interactable = false;
        ScaleEffect(oldItem.GetComponent<RectTransform>());
        oldItem.button.onClick.AddListener(OnClickedOldItem);
        newItem.button.onClick.AddListener(OnClickedNewItem);
    }

    void OnClickedNewItem()
    {
        FF_TutorialManager.Instance.tutorialText.text =
            StringManager.Instance.GetText(inventoryCompleteKey);

        inventory.gameObject.SetActive(false);
        Destroy(bubble.gameObject);
        onClick.AddListener(OnTutorialStepComplete);
    }

    void OnClickedOldItem()
    {
        newItem.button.interactable = true;

        FF_TutorialManager.Instance.tutorialText.text =
            StringManager.Instance.GetText(inventoryRetryKey);

        oldItem.button.interactable = false;
        ScaleEffect(newItem.GetComponent<RectTransform>());
    }

    public void ShowToggle()
    {
        plantModeToggle.SetActive(true);
        onClick.RemoveAllListeners();

        FF_TutorialManager.Instance.tutorialText.text =
            StringManager.Instance.GetText(toggleHouseModeKey);

        Sequence toggleSequence = DOTween.Sequence();
        toggleSequence.Append(ScaleEffect(plantModeToggle.GetComponent<RectTransform>()));
        toggleSequence.onComplete += () =>
        {
            plantModeToggle.GetComponent<Toggle>().interactable = true;
        };
    }

    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 0.5f)
                         .SetLoops(4, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }

    void SwitchToHouseMode()
    {
        plantModeToggle.GetComponent<Toggle>().interactable = false;
        HH_GameManager.Instance.IsPlantMode = false;

        FF_TutorialManager.Instance.tutorialText.text =
            StringManager.Instance.GetText(upgradeRoofIntroKey);

        canClick = false;

        HH_GameManager.Instance.cameraController.Zoomcamera(camH1, true, 60);
        houseManager = HH_GameManager.Instance.currentPlayer;
        partObjects = houseManager.GetAllHousePartObjectsOf(partType);

        foreach (var part in partObjects)
        {
            if (part.shouldDisplayBubble)
            {
                bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
                bubble.InitBubbleForHousePart(part);
                part.bubble = bubble;
                bubble.button.enabled = false;
            }
            break;
        }

        ShowUpgradeHousePart();
    }

    public void ShowUpgradeHousePart()
    {
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnPartTapped;

        bubble.button.enabled = true;
        bubble.button.onClick.AddListener(() =>
        {
            OnPartTapped(gameObject);
        });

        highlightMesh.HighlightMeshes();

        DOVirtual.DelayedCall(1f, () =>
        {
            foreach (var part in partObjects)
            {
                part.isClickable = true;
                highlightMesh.meshRenders.AddRange(part.meshes);
            }
        });
    }

    void OnPartTapped(GameObject obj)
    {
        var part = obj.GetComponentInParent<BaseHousePartObject>();

        if (part && part.HousePartType == partType)
        {
            FF_TutorialManager.Instance.tutorialText.text =
                StringManager.Instance.GetText(purchaseMaterialKey);
        }
    }
}
