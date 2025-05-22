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
            if (IsPointerOverUI(Input.mousePosition))
            {
                //Debug.Log("Pointer is over a UI element. Skipping raycast.");
                return;
            }
            StartCoroutine(DelayedRaycast(Input.mousePosition));

        }

        IEnumerator DelayedRaycast(Vector2 screenPos)
        {
            yield return null; // Wait one frame

            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.collider.CompareTag("House") && canClickHouse)
                {
                    if (hit.collider.transform.parent.CompareTag("Fence")) yield break;
                    var house = hit.collider.GetComponent<HouseManager>();
                    OnHouseSelected?.Invoke(house);
                }

                if (canClickHouse) yield break;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Structure"))
                {
                    if (HH_GameManager.Instance.IsPlantMode) yield break;
                    if (HH_GameManager.Instance.IsGameStarted || HH_GameManager.Instance.isTutorial)
                    {
                        OnObjectSelected?.Invoke(hit.collider.gameObject);
                    }
                    Debug.Log($"Clicked {hit.collider.gameObject.name}");
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Nature") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Combustible"))
                {
                    //Debug.Log($"Hit {hit.collider.gameObject}");
                    OnObjectSelected?.Invoke(hit.collider.gameObject);
                }
            }
        }

    }

    public bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
