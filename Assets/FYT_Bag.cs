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
    public TextMeshProUGUI bagList4;
    private string list;
    private string list2;
    private string list3;
    private int count = 0;
    public List<string> collectedItems = new List<string>();

    //Old Text Strings of the Bag Panel
    /*private List<string> col1 = new List<string>();
    private List<string> col2 = new List<string>();
    private List<string> col3 = new List<string>();
    private List<string> col4 = new List<string>();*/

    public GameObject itemPrefab;
    public Transform collectedContainer;

    public GameObject siren;
    public GameObject timer;
    public FYT_ResultsScreen resultsScreen;

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
        LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(string newLang)
    {
        if (this == null || bagPanel == null) return;

        RefreshBagText();
        /*if (this != null && bagPanel != null && bagPanel.activeInHierarchy)
        {
            RefreshBagText();
        }*/
        
    }

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
        RefreshBagText();
    }

   public void RefreshBagText()
    {
        
        foreach (Transform child in collectedContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < collectedItems.Count; i++)
        {
            string key = collectedItems[i];

            if (key == "Salem's Vet Records" || key == "Salem’s Vet Records") key = "catVetText";

            string translated = key;

            if (StringManager.Instance != null)
            {
                translated = StringManager.Instance.GetText(key);

                if (string.IsNullOrEmpty(translated) || translated.Contains("[Missing"))
                {
                    string fixedKey = key.Replace(" ", "") + "Text";
                    fixedKey = char.ToLower(fixedKey[0]) + fixedKey.Substring(1);
                    translated = StringManager.Instance.GetText(fixedKey);
                }
            }

            GameObject entry = Instantiate(itemPrefab, collectedContainer);
            var textComp = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = translated;
            }
        }
        /*col1.Clear();
        col2.Clear();
        col3.Clear();
        col4.Clear();

        for (int i = 0; i < collectedItems.Count; i++)
        {
            string key = collectedItems[i];

            if (key == "Salem's Vet Records" || key == "Salem’s Vet Records") key = "catVetText";

            string translated = key;

            if (StringManager.Instance != null)
            {
                translated = StringManager.Instance.GetText(key);

                if (string.IsNullOrEmpty(translated) || translated.Contains("[Missing"))
                {
                    string fixedKey = key.Replace(" ", "") + "Text";
                    fixedKey = char.ToLower(fixedKey[0]) + fixedKey.Substring(1);
                    translated = StringManager.Instance.GetText(fixedKey);
                }
            }
            //18, then 36
            if (i < 32) col1.Add(translated);
            else if (i < 64) col2.Add(translated);
            else if (i < 96) col3.Add(translated);
            else col4.Add(translated);
        }

        bagList.text = string.Join("\n", col1);
        bagList2.text = string.Join("\n", col2);
        bagList3.text = string.Join("\n", col3);
        bagList4.text = string.Join("\n", col4);*/
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
        collectedItems.Add(item);
        RefreshBagText();
        //Original Bag Code
        /*if (!bagList.text.Contains(item) && !bagList2.text.Contains(item) && !bagList3.text.Contains(item))
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
        }*/
    }
}
