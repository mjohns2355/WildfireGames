using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewLoseScript : MonoBehaviour
{
    public TextMeshProUGUI countText;

    [Header("Items Collected")]
    public GameObject itemPrefab;
    public Transform collectedContainer;

    //Checklist Assets
    public Sprite greenContainer, redContainer;

    private string Translate(string key)
    {
        if (StringManager.Instance != null)
        {
            if (key == "Salem's Vet Records") key = "catVetText";
            string translated = StringManager.Instance.GetText(key);

            if (string.IsNullOrEmpty(translated) || translated.Contains("[Missing"))
            {
                string fixedKey = key.Replace(" ", "") + "Text";
                fixedKey = char.ToLower(fixedKey[0]) + fixedKey.Substring(1);
                translated = StringManager.Instance.GetText(fixedKey);
            }
            if (!string.IsNullOrEmpty(translated) && !translated.Contains("[Missing"))
                return translated;
        }
        return key;
    }

    public void Show(List<string> allCollected)
    {
        // Stars taken from the FR + FF code base
        //UpdateStarVisuals(FYT_ScoreData.starRating, starImages);
        /*int fullStars = (int)FYT_ScoreData.starRating;
        bool hasHalf = (FYT_ScoreData.starRating - fullStars) >= 0.5f;

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(i < fullStars);
        }

        if (halfStarImage != null)
        {
            halfStarImage.SetActive(hasHalf);
        }*/

        //UpdateFeedbackText(FYT_ScoreData.starRating);
        // Count
        //countText.text = $"{FYT_ScoreData.essentialCollected} / {FYT_ScoreData.essentialTotal} essential items";
        string localizedFormat = Translate("essentialItemsText");
        countText.text = string.Format(localizedFormat, FYT_ScoreData.essentialCollected, FYT_ScoreData.essentialTotal);

        // Left column — all collected items
        foreach (string item in allCollected)
        {
            GameObject entry = Instantiate(itemPrefab, collectedContainer);
            var textComp = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = Translate(item);
            }
        }

    }
}
