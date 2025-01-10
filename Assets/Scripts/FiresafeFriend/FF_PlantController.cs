using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_PlantController : MonoBehaviour
{
    public Transform plantHolder;
    public FF_Plants currentPlant;
    public Transform bubblePos;

    PurchaseFloatingButton bubble;
    GameObject[] plantPrefabs;
    // Start is called before the first frame update
    void Start()
    {        bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
        bubble.SetTargetPosition(bubblePos.position);
        bubble.SetPlantIcon(!(currentPlant == null));
        plantPrefabs = ResourceManager.Instance.plants;
        var rng = Random.Range(0, 1f);
        if(rng < 0.1)
        {
            var index = Random.Range(0,plantPrefabs.Length);
            currentPlant = Instantiate(plantPrefabs[index],plantHolder).GetComponent<FF_Plants>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
