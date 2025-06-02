using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HappyHouse.HouseSystem;
public class FF_DirtMound : MonoBehaviour
{
    public Transform plantHolder;
    public FF_Plants currentPlant;
    public Transform bubblePos;
    public Vector3 menuPos;
    //public List<FF_Plants> ownedPlants = new List<FF_Plants>();
    public List<FF_Plants> availablePlants;
    public Action<FF_Plants> OnPlanted;
    public Action OnShoveled;
    public HouseManager owner;
    PurchaseFloatingButton bubble;
  
    // Start is called before the first frame update
    void Start()
    {        
        bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
        bubble.InitBubbleForPlant(this, !(currentPlant == null), bubblePos.position);
        availablePlants = new List<FF_Plants>(ResourceManager.Instance.plants);
        if (!HH_GameManager.Instance.isTutorial)
        {
            RandomizeInitialPlant();
        }

        if (currentPlant == null)
        {
            SetBubbleState(false);
        }
        HH_GameManager.Instance.OnPlantModeChanged += (isPlantMode) =>
        {
            if (currentPlant == null)
            {
                SetBubbleState(isPlantMode);
            }
            else
            {
                currentPlant.isClickable = isPlantMode;
                
            }
        };

        HH_GameManager.Instance.OnRoundEnd += () =>
        {
            if (owner && currentPlant)
            {
                owner.ownedPlants.Add(currentPlant);
            }
        };

    }

    public void Plant(FF_Plants plant)
    {
        //shovel existing plant
        Shovel();
        currentPlant = Instantiate(plant, plantHolder);
        currentPlant.onPlantClicked += ()=> HH_GameManager.Instance.uiManager.ShowPlantsMenu(this);
        availablePlants.Remove(plant);
        bubble.gameObject.SetActive(false);
        currentPlant.isClickable = true;
        OnPlanted?.Invoke(plant);
    }

    public void RandomizeInitialPlant()
    {
        var rng = UnityEngine.Random.Range(0, 1f);
        if (rng < 0.1)
        {
            var index = UnityEngine.Random.Range(0, availablePlants.Count);
            var p = availablePlants[index];
            Plant(p);
            currentPlant.isClickable = false;
        }
    }
    public void Shovel()
    {
        if (currentPlant != null)
        {
            HH_GameManager.Instance.uiManager.HidePlantsMenu();
            
            bubble.gameObject.SetActive(true);
            foreach (var p in ResourceManager.Instance.plants)
            {
                if (p.combustibleInfo.partID == currentPlant.combustibleInfo.partID)
                {
                    availablePlants.Add(p);
                    break;
                }
            }
            var plantToDestroy = currentPlant;
            plantToDestroy.onPlantClicked = null;
            currentPlant = null;
            OnShoveled?.Invoke();
            Destroy(plantToDestroy.gameObject);
        }
    }

    public void SetBubbleState(bool state)
    {
        bubble.gameObject.SetActive(state);

    }
    private void OnMouseDown()
    {
        if (!HH_GameManager.Instance.IsPlantMode || EventSystem.current.IsPointerOverGameObject()) return;
        HH_GameManager.Instance.uiManager.ShowPlantsMenu(this);
    }


}
