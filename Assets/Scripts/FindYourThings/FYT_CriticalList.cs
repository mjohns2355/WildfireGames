using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FYT_CriticalList : MonoBehaviour
{
    public GameObject listOfItems;
    public GameObject medsText;
    public GameObject glassesText;
    public GameObject endMenu;
    private List<GameObject> collectedItems;

    // Start is called before the first frame update
    void Start()
    {
        if (FYT_SettingsData.medsNeeded == true)
        {
            medsText.SetActive(true);
        }
        if (FYT_SettingsData.glassesNeeded == true)
        {
            glassesText.SetActive(true);
        }
        collectedItems = new List<GameObject>();
    }

    // Update is called once per frame
    public void crossOffList(string item)
    {
        GameObject collectable = GameObject.Find(item);

        foreach (Transform child in listOfItems.transform)
        {
            TextMeshProUGUI childName = child.gameObject.GetComponent<TextMeshProUGUI>();
            if (collectable.name == childName.text)
            {
                childName.fontStyle = FontStyles.Strikethrough;
                childName.color = Color.gray;
            }
        }

        collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Remove(collectable);
        collectedItems.Add(collectable);
        collectable.SetActive(false);
    }

    public void endList()
    {
        FYT_SettingsData.finalInventory = collectedItems;
        endMenu.SetActive(true);
    }
}
