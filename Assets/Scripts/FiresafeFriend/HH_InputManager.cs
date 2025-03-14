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
        if (HH_GameManager.Instance.IsFireStarted) return;
        DetectInput();
    }

    void DetectInput()
    {
        // Handle mouse click for PC
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(DelayedRaycast(Input.mousePosition));
        }

    }

    IEnumerator DelayedRaycast(Vector2 screenPos)
    {
        yield return null; // Wait one frame

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Pointer is over a UI element. Raycast blocked.");
            yield break;
        }
            

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (/*hit.collider.CompareTag("House")*/hit.collider.gameObject.layer == LayerMask.NameToLayer("Structure"))
            {
                if (canClickHouse)
                {
                    if (hit.collider.transform.parent.CompareTag("Fence")) yield break;
                    var house = hit.collider.transform.parent.GetComponentInParent<HouseManager>();
                    OnHouseSelected?.Invoke(house);
                }
                else
                {
                    if (HH_GameManager.Instance.IsGameStarted || HH_GameManager.Instance.isTutorial)
                    {
                        OnObjectSelected?.Invoke(hit.collider.gameObject);
                    }
                    //Debug.Log($"Clicked {hit.collider.gameObject.name}");

                }
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Nature") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Combustible"))
            {
                //Debug.Log($"Hit {hit.collider.gameObject}");
                OnObjectSelected?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
