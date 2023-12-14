using System.Collections;
using UnityEngine;

public class FYT_PetManager : MonoBehaviour
{
    /* Controls the pet's spawning and movement */
    public TMPro.TextMeshProUGUI inventoryText;
    public GameObject petObject;
    public GameObject petPrefab;
    public GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
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
        petPrefab.SetActive(true);
        petObject.SetActive(false);
    }
}
