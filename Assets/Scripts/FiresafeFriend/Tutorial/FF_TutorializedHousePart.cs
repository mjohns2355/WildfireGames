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
            foreach(var part in partObjects)
            {
                part.isClickable = false;
            }
            HH_GameManager.Instance.inputManager.OnObjectSelected -= OnPartTapped;
            bubble.gameObject.SetActive(false);
            //Destroy(bubble.gameObject);
            ShowInventoryButton();
            //OnTutorialStepComplete();
        });
        onClick.AddListener(ShowToggle);
    }
    public void ShowInventoryButton()
    {
        FF_TutorialManager.Instance.tutorialText.text = "You can view your Owned Materials in the Inventory. Try tapping on the Inventory button to open the Inventory";
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
        FF_TutorialManager.Instance.tutorialText.text = "You can always tap on the item to replace the material. Try tapping on the Wood";

        oldItem = inventory.items[0];
        newItem = inventory.items[1];
        newItem.button.interactable = false;
        ScaleEffect(oldItem.GetComponent<RectTransform>());
        oldItem.button.onClick.AddListener(OnClickedOldItem);
        newItem.button.onClick.AddListener(OnClickedNewItem);
    }

    void OnClickedNewItem()
    {
        FF_TutorialManager.Instance.tutorialText.text = "Now your roof is using Metal. You can use the inventory anytime you want to apply other materials to your house!";
        inventory.gameObject.SetActive(false);
        Destroy(bubble.gameObject);
        onClick.AddListener(OnTutorialStepComplete);
    }

    void OnClickedOldItem()
    {
        newItem.button.interactable = true;
        FF_TutorialManager.Instance.tutorialText.text = "Great job! Now try tapping on the Metal again";
        oldItem.button.interactable = false;
        ScaleEffect(newItem.GetComponent<RectTransform>());
    }
    public void ShowToggle()
    {
        plantModeToggle.SetActive(true);
        onClick.RemoveAllListeners();
        FF_TutorialManager.Instance.tutorialText.text = "Click on the toggle to switch to “House Mode”";
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
        FF_TutorialManager.Instance.tutorialText.text = "Let’s try upgrading the roof. Tap to see the roofing options.";
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
        if (part)
        {         
            if (part.HousePartType == partType)
            {
                FF_TutorialManager.Instance.tutorialText.text = "Each material is graded for fire resistance, with Class A being the highest rating. Let’s purchase this metal roofing.";
            }
        }

    }



}
