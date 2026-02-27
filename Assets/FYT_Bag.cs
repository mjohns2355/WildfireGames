using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FYT_Bag : MonoBehaviour
{
    public GameObject bagPanel;
    public TextMeshProUGUI packText;
    public TextMeshProUGUI bagList;
    public TextMeshProUGUI bagList2;
    public TextMeshProUGUI bagList3;
    private string list;
    private string list2;
    private string list3;
    private int count = 0;
    private List<string> collectedItems = new List<string>();

    public GameObject siren;
    public GameObject timer;
    public FYT_ResultsScreen resultsScreen;

    public void OpenBag()
    {
        bagPanel.SetActive(true);
        string packedHeader = "Packed:";

        if (StringManager.Instance != null)
        {
            string foundText = StringManager.Instance.GetText("packedText");
            
            if (!string.IsNullOrEmpty(foundText) && !foundText.Contains("[Missing"))
            {
                packedHeader = foundText;
            }
        }

        packText.text = packedHeader;
    }

    public void Evac()
    {
        FYT_ScoreCalculator.Calculate(collectedItems);
        Debug.Log($"[FYT] Stars: {FYT_ScoreData.starRating} | Collected: {FYT_ScoreData.essentialCollected}/{FYT_ScoreData.essentialTotal}");
        resultsScreen.gameObject.SetActive(true);
        resultsScreen.Show(collectedItems);
        Destroy(siren);
        Destroy(timer);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void AddCatalogItem(string catalogName)
    {
        collectedItems.Add(catalogName);
    }

    public void AddItem(string item)
    {
        if (!bagList.text.Contains(item) && !bagList2.text.Contains(item) && !bagList3.text.Contains(item))
        {
            count++;

            string translatedItem = (StringManager.Instance != null) ? StringManager.Instance.GetText(item) : item;
            //packedText
            
            if (count <= 18)
            {
                list += "\n" + translatedItem;
                bagList.text = list;
            }
            else if (count <= 36)
            {

                list2 += "\n" + translatedItem;
                bagList2.text = list2;
            }
            else
            {

                list3 += "\n" + translatedItem;
                bagList3.text = list3;
            }
        }
    }
}
