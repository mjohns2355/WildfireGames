using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_PlantsMenu : MonoBehaviour
{
    public Transform grid;
    public GameObject plantOptionPrefab;
    [SerializeField] Vector3 posOffset;
    FF_DirtMound currentOwner;
    Vector3 pos;
    float baseDistance;
    // Start is called before the first frame update
    void Start()
    {
        baseDistance = Vector3.Distance(Camera.main.transform.position, pos);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 adjustedOffset = Vector3.zero;
        if (HH_GameManager.Instance.IsPlantMode)
        {
            //float fovScale = Mathf.Clamp(Camera.main.fieldOfView / 60, 0.5f, 1.2f);
            //adjustedOffset = posOffset * fovScale;
            float distance = Vector3.Distance(Camera.main.transform.position, pos);
            float distanceScale = Mathf.Clamp(distance / baseDistance, 0.2f, 0.8f);

            adjustedOffset = posOffset * distanceScale;
        }
        //else
        //{
        //    float distance = Vector3.Distance(Camera.main.transform.position, pos);
            
        //    float distanceScale = Mathf.Clamp(distance / baseDistance, 0.5f, 1.2f);

        //    adjustedOffset = posOffset * distanceScale;
        //}

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos + adjustedOffset);
        float padding = 50f; 
        screenPos.x = Mathf.Clamp(screenPos.x, padding, Screen.width - padding);
        screenPos.y = Mathf.Clamp(screenPos.y, padding, Screen.height - padding);

        transform.position = screenPos;
        //Vector3 cameraForward = Camera.main.transform.forward;
        //cameraForward.y = 0; // Keep it upright (prevents unwanted tilting)

        //transform.rotation = Quaternion.LookRotation(cameraForward);
    }

    public void ShowPlantsMenu(FF_DirtMound owner)
    {
        ClearOptions();
        currentOwner = owner;
        pos = owner.bubblePos.position;
        currentOwner.OnPlanted += _ => OnPlanted();
        PopulateOptions();
    }

    public void ClosePlantsMenu()
    {
        if(currentOwner != null)
        {
            currentOwner.OnPlanted -= _ => OnPlanted();
            currentOwner = null;
        }

        ClearOptions();
        gameObject.SetActive(false);
        
    }
    private void PopulateOptions()
    {
        foreach (var p in currentOwner.availablePlants)
        {
            var option = Instantiate(plantOptionPrefab, grid).GetComponent<FF_PlantMenuOption>();
            option.InitPlantMenuOption(p,currentOwner);
        }
        var removeButton = Instantiate(plantOptionPrefab, grid).GetComponent<FF_PlantMenuOption>();
        removeButton.InitRemoveButton(currentOwner);
    }

    private void ClearOptions()
    {
        for(int i = 0; i< grid.childCount; i++)
        {
            Destroy(grid.GetChild(i).gameObject);
        }
    }

    private void OnPlanted()
    {
        
        ClosePlantsMenu();
        //ClearOptions();
        //PopulateOptions() ;
    }
    
    public void RefreshLanguage()
    {
        if (!gameObject.activeInHierarchy) return;

        FF_PlantMenuOption[] options = grid.GetComponentsInChildren<FF_PlantMenuOption>();

        foreach (var option in options)
        {
            option.RefreshLanguage();
        }
    }
}
