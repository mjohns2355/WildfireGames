using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class HH_InputManager : MonoBehaviour
{
    public UnityEvent <BaseHousePartObject> OnHousePartSelected;
    public Action<HouseManager> OnHouseSelected;
    public Action<GameObject> OnObjectSelected;
    public bool canClickHouse = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();
    }

    void DetectInput()
    {
        // Handle mouse click for PC
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Pointer is over a UI element. Raycast blocked.");
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (/*hit.collider.CompareTag("House")*/hit.collider.gameObject.layer == LayerMask.NameToLayer("Structure"))
                {
                    if (canClickHouse )
                    {
                        if (hit.collider.transform.parent.CompareTag("Fence")) return;
                        var house = hit.collider.transform.parent.GetComponentInParent<HouseManager>();
                        OnHouseSelected?.Invoke(house);
                    }
                    else
                    {
                        if(!HH_GameManager.Instance.IsGameStarted) return;
                        Debug.Log($"Clicked {hit.collider.gameObject.name}");
                        OnObjectSelected?.Invoke(hit.collider.gameObject);
                    }
                }

                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Nature") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Combustible"))
                {
                    Debug.Log($"Hit {hit.collider.gameObject}");
                    OnObjectSelected?.Invoke(hit.collider.gameObject);
                }
                //if(hit.collider.gameObject.layer == 10)
                //{
                //    PurchaseFloatingButton bubble = hit.collider.GetComponentInParent<BaseHousePartObject>().bubble;
                //    if (bubble != null)
                //    {
                //        bubble.OnBubbleClicked();
                //    }
                //}

            }
        }

        // Handle touch input for mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {            
            // Check if the touch is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                //Debug.Log("Touch is over a UI element. Raycast blocked.");
                return;
            }
            Vector3 touchPosition = Input.GetTouch(0).position;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (/*hit.collider.CompareTag("House")*/hit.collider.gameObject.layer == LayerMask.NameToLayer("Structure"))
                {
                    if (canClickHouse)
                    {
                        if (hit.collider.transform.parent.CompareTag("Fence")) return;
                        var house = hit.collider.transform.parent.GetComponentInParent<HouseManager>();
                        OnHouseSelected?.Invoke(house);
                    }
                    else
                    {
                        Debug.Log($"Clicked {hit.collider.gameObject.name}");
                        OnObjectSelected?.Invoke(hit.collider.gameObject);
                    }
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Nature"))
                {
                    OnObjectSelected?.Invoke(hit.collider.gameObject);
                }
            }
        }
    }
}
