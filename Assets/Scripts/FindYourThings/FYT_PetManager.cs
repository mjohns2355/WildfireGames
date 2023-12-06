using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_PetManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI inventoryText;
    private RectTransform inventoryRect;
    public GameObject petObject;
    public GameObject petPrefab;
    public GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        inventoryRect = inventoryText.gameObject.GetComponent<RectTransform>();
        petObject.SetActive(true);
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (petObject.activeSelf)
        {
            Transform newPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            petObject.transform.SetParent(null);
            petObject.transform.position = new Vector2(newPoint.position.x, newPoint.position.y);
            petObject.transform.SetParent(newPoint);
            yield return new WaitForSeconds(1);
        }
    }

    public void addPet()
    {
        GameObject petItem = Instantiate(petPrefab, inventoryRect);
        RectTransform petRect = petItem.GetComponent<RectTransform>();
        petRect.anchoredPosition = new Vector2(-370, -85);
        petObject.SetActive(false);
    }
}
