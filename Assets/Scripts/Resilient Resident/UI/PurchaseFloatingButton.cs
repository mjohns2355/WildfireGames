using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseFloatingButton : MonoBehaviour
{
    public BaseHousePartObject owner;
    public Image iconImage;
    public Vector3 offset;

    private Camera mainCamera;
    [SerializeField] private Button button;

    // Static reference to the currently selected bubble button
    private static PurchaseFloatingButton currentSelectedButton;

    void Start()
    {
        mainCamera = Camera.main;
        button.onClick.AddListener(OnBubbleClicked);
    }

    public void OnBubbleClicked()
    {
        if (currentSelectedButton == this)
        {
            // If this button is already selected, deselect it and close the store panel
            HH_GameManager.Instance.uiManager.HideStoreScreen();
            ResetButton();
            currentSelectedButton = null;
        }
        else
        {
            // Deselect the previously selected button, if any
            if (currentSelectedButton != null)
            {
                currentSelectedButton.ResetButton();
            }

            // Select this button and open the store panel
            currentSelectedButton = this;
            SelectButton();
            HH_GameManager.Instance.uiManager.ShowStoreScreen(owner.partInfo, this);
        }
    }

    void Update()
    {
        if (owner != null && iconImage != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(owner.transform.position + offset);
            iconImage.transform.position = screenPos;
            iconImage.enabled = screenPos.z > 0;
        }
    }
    public void InitBubble(BaseHousePartObject ownerPart)
    {
        owner = ownerPart;
        //HH_GameManager.Instance.inputManager.OnHousePartSelected.AddListener(OnHousePartClicked);
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
