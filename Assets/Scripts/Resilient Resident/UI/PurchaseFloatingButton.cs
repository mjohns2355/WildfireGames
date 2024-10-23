using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PurchaseFloatingButton : MonoBehaviour
{
    public BaseHousePartObject owner; 
    public Image iconImage; 
    public Vector3 offset; 

    private Camera mainCamera;
    private Button button;
    [SerializeField]private bool isSelected = false;

    void Start()
    {
        mainCamera = Camera.main;
        button = GetComponent<Button>();
        button.onClick.AddListener(

            () =>
            {
                
                if (isSelected)
                {
                    HH_GameManager.Instance.UIManager.HideStoreScreen();
                    ResetButton();
                }
                else
                {
                    HH_GameManager.Instance.UIManager.ShowStoreScreen(owner, this);
                    SelectButton();
                }

            }

        );

    }

    void Update()
    {
        if (owner != null && iconImage != null)
        {
            
            Vector3 screenPos = mainCamera.WorldToScreenPoint(owner.transform.position + offset);

            if (screenPos.z > 0)
            {
                iconImage.transform.position = screenPos;
                iconImage.enabled = true; 
            }
            else
            {
                iconImage.enabled = false;
            }
        }
    }

    public void ResetButton()
    {
        isSelected = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectButton()
    {
        isSelected = true;
        button.Select();
    }
}
