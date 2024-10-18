using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class CraftIcon : MonoBehaviour
{
    public BaseHousePartObject owner; 
    public Image iconImage; 
    public Vector3 offset; 

    private Camera mainCamera;
    private Button button;
    void Start()
    {
        mainCamera = Camera.main;
        button = GetComponent<Button>();
        button.onClick.AddListener(HH_GameManager.Instance.UIManager.ShowStoreScreen);
        
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
}
