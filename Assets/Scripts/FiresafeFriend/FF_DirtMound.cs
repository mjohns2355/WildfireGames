using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_DirtMound : MonoBehaviour
{
    public Transform plantHolder;
    public FF_Plants currentPlant;
    public Transform bubblePos;
    public Vector3 menuPos;
    //public List<FF_Plants> ownedPlants = new List<FF_Plants>();
    public List<FF_Plants> availablePlants;
    public Action OnPlanted;
    public Action OnShoveled;
    PurchaseFloatingButton bubble;
  
    // Start is called before the first frame update
    void Start()
    {        
        bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
        bubble.InitBubbleForPlant(this, !(currentPlant == null), bubblePos.position);
        //menuPos = bubblePos.position + new Vector3(0, 15f , 0);
        //bubble.SetTargetPosition(bubblePos.position);
        //bubble.SetPlantIcon(!(currentPlant == null));
        availablePlants = new List<FF_Plants>(ResourceManager.Instance.plants);
        if (!HH_GameManager.Instance.isTutorial)
        {
            var rng = UnityEngine.Random.Range(0, 1f);
            if (rng < 0.1)
            {
                var index = UnityEngine.Random.Range(0, availablePlants.Count);
                var p = availablePlants[index];
                //currentPlant = Instantiate(p, plantHolder);
                //availablePlants.Remove(p);
                Plant(p);
            }
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
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Plant(FF_Plants plant)
    {
        //shovel existing plant
        Shovel();
        //Debug.Log("plant");
        currentPlant = Instantiate(plant, plantHolder);

        //currentPlant.isClickable = false;
        availablePlants.Remove(plant);
        //bubble.SetPlantIcon(true);
        bubble.gameObject.SetActive(false);
        //var newTargetPos = new Vector3(bubblePos.position.x, currentPlant.topPosition.y + 4f, bubblePos.position.z);
        //bubble.SetTargetPosition(newTargetPos);
        //menuPos = newTargetPos + new Vector3(0, currentPlant.topPosition.y + 15f, 0);
        currentPlant.onPlantClicked += () => HH_GameManager.Instance.uiManager.ShowPlantsMenu(this);
        OnPlanted?.Invoke();
    }

    public void Shovel()
    {
        if (currentPlant != null)
        {
            //Debug.Log("shovel");
            //availablePlants.Add(currentPlant);
            //menuPos = bubblePos.position + new Vector3(0, 15f, 0);

            //bubble.SetTargetPosition(bubblePos.position);
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
            Destroy(plantToDestroy.gameObject);
            OnShoveled?.Invoke();
        }
    }

    void SetBubbleState(bool state)
    {
        bubble.gameObject.SetActive(state);
    }


}
