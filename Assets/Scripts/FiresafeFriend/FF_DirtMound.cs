using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_DirtMound : MonoBehaviour
{
    public Transform plantHolder;
    public FF_Plants currentPlant;
    public Transform bubblePos;
    //public List<FF_Plants> ownedPlants = new List<FF_Plants>();
    public List<FF_Plants> availablePlants;
    PurchaseFloatingButton bubble;
  
    // Start is called before the first frame update
    void Start()
    {        
        bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
        bubble.SetTargetPosition(bubblePos.position);
        bubble.SetPlantIcon(!(currentPlant == null));
        availablePlants= ResourceManager.Instance.plants;
        var rng = Random.Range(0, 1f);
        if(rng < 0.1)
        {
            var index = Random.Range(0, availablePlants.Count);
            var p = availablePlants[index];
            currentPlant = Instantiate(p, plantHolder);
            availablePlants.Remove(p);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
