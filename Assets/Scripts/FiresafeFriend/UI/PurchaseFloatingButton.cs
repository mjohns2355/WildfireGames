using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseFloatingButton : MonoBehaviour
{
    public BaseHousePartObject ownerPart;
    public FF_DirtMound ownerMound;
    public Image iconImage;
    public Vector3 offset;
    
    private Camera mainCamera;
    private bool isPlant = false;
    private bool shouldShowRemoveIcon = false;
    private Vector3 targetPosition;
    [SerializeField] private Button button;
    [SerializeField] Sprite purchase, plant;

    // Static reference to the currently selected bubble button
    private static PurchaseFloatingButton currentSelectedButton;

    void Start()
    {
        mainCamera = Camera.main;
        button.onClick.AddListener(OnBubbleClicked);
    }

    public void OnBubbleClicked()
    {
        //if (shouldShowRemoveIcon)
        //{
        //    ownerMound.Shovel();
        //    shouldShowRemoveIcon = false;
        //    SetPlantIcon(false);
        //    return;
        //}

        if (isPlant)
        {
            HH_GameManager.Instance.uiManager.ShowPlantsMenu(ownerMound);
        }
        else
        {
            HH_GameManager.Instance.uiManager.ShowStoreScreen(ownerPart.partInfo.housePartType, this);
        }
        
    }

    void Update()
    {
        //if (ownerPlant == null & ownerPart == null) return;
        if (iconImage != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(/*ownerPart.bubblePos.position*/ targetPosition /*+ offset*/);
            iconImage.transform.position = screenPos;
            iconImage.enabled = screenPos.z > 0;
        }
    }
    public void InitBubbleForHousePart(BaseHousePartObject ownerPart)
    {
        this.ownerPart = ownerPart;
        iconImage.sprite = purchase;
        targetPosition = ownerPart.bubblePos.position;
        //HH_GameManager.Instance.inputManager.OnHousePartSelected.AddListener(OnHousePartClicked);
    }

    public void InitBubbleForPlant(FF_DirtMound owner, bool isPlanted, Vector3 targetPosition)
    {
        ownerMound = owner;
        isPlant = true;
        SetPlantIcon(isPlanted);
        SetTargetPosition(targetPosition);
    }

    public void SetTargetPosition (Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public void SetPlantIcon(bool isPlanted)
    {
        //if (isPlanted)
        //{
        //    iconImage.sprite = remove;
        //    shouldShowRemoveIcon = true;
        //    return;
        //}
        iconImage.sprite = plant;
    }
    public void ResetButton()
    {
        currentSelectedButton = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}
