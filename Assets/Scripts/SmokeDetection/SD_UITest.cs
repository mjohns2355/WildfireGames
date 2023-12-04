using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SD_UITest : MonoBehaviour
{
    int UILayer;
    [SerializeField] GameObject doubleClickHold; //Checks if you click twice
    

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
    }

    private void Update()
    {
        //will check what you are currently hovering
        GameObject checkHoveredObject = GetHoveredObject();

        if(checkHoveredObject != null) //checks if its a pickup object
        {
            if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                ClickedHoveredItem(checkHoveredObject);
            }
        }
        if(checkHoveredObject == null) //checks if u got nothing
        {
            if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                ClickedOffItem();
            }
        }

    }

    // Returns the object being hovered over with the specified tag.
    private GameObject GetHoveredObject()
    {
        List<RaycastResult> raycastResults = GetEventSystemRaycastResults();
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.layer == UILayer)
            {
                return result.gameObject;
            }
        }
        return null;
    }
    
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (Input.touchCount > 0)
        {
            eventData.position = Input.GetTouch(0).position;
        }
        else
        {
            eventData.position = Input.mousePosition;
        }
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }

    // Show the ToolTip of the object.
    private void ShowToolTip(GameObject obj)
    {
        if (obj != null)
        {
            Transform toolTip = obj.transform.Find("ToolTip");
            if (toolTip != null)
            {
                toolTip.gameObject.SetActive(true);
            }
        }
    }

    // Hide the ToolTip of the object.
    private void HideToolTip(GameObject obj)
    {
        if (obj != null)
        {
            Transform toolTip = obj.transform.Find("ToolTip");
            if (toolTip != null)
            {
                toolTip.gameObject.SetActive(false);
            }
        }
    }
    private void ClickedHoveredItem(GameObject obj)
    {
        if(obj.CompareTag("PickupObject") == true)
        {
            pickupItem(obj);
        }
        if(obj.CompareTag("InteractableObject") == true)
        {
            interactObject(obj);
        }
        // else
        // {
        //     HideToolTip(doubleClickHold);
        //     if (obj != null)
        //     {
        //         doubleClickHold = obj;
        //         ShowToolTip(obj);
        //     }
        // }
    }
    private void ClickedOffItem()
    {
        HideToolTip(doubleClickHold);
        doubleClickHold = null;
    }


    // RECHANGE FOR OTHER
    private void pickupItem(GameObject item)
    {
        Animation itemAnimation = item.GetComponent<Animation>();
        SD_Inventory playerInventory = SD_Inventory.Instance;
        if(itemAnimation != null)
        {
            itemAnimation.Play();
            StartCoroutine(WaitForAnimation(itemAnimation, item));
        }
        else
        {
            item.SetActive(false);
            playerInventory.AddItem(item);
        }

        
    }

    private void interactObject(GameObject obj)
    {
        SD_UISwitchObject switchObject = obj.GetComponent<SD_UISwitchObject>();

        if (switchObject != null)
        {
            switchObject.UseItemToSwitch();
        }
    }
    private IEnumerator WaitForAnimation(Animation animation, GameObject item)
    {
        while (animation.isPlaying)
        {
            yield return null;
        }
        item.SetActive(false);
        SD_Inventory.Instance.AddItem(item);
    }
}
