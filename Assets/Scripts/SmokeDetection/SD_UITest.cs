using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SD_UITest : MonoBehaviour
{
    int UILayer;
    [SerializeField] GameObject doubleClickHold; //Checks if you click twice
    [SerializeField] private GameObject TVUIPopup;
    [SerializeField] private GameObject TV;
    private float currentTimer = 0f;
    private bool startTimer;
    private int counting = 0;

    

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
        if(startTimer == true && counting <= 2)
        {
            TVTimer();
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
        if(obj.CompareTag("msgPopup") == true)
        {
            interactPopup(obj);
        }
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

    private void interactPopup(GameObject obj)
    {
        obj.SetActive(false);
        SD_GameSateManager.Instance.switchGameState(SD_GameState.Paused);
        TVUIPopup.SetActive(true);
        startTimer = false;
        currentTimer = 0f;
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

    public void TVTimer()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer >= 10f) //CHANGE
        {
            TV.SetActive(true);
        }
    }
    public void startTVTimer()
    {
        startTimer = true;
    }
    public void TVcounter()
    {
        if(counting < 2)
        {
            counting++;
        }
    }
}
