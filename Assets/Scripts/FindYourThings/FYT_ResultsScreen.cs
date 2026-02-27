using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYT_ResultsScreen : MonoBehaviour
{
    [Header("Middle Column")]
    public GameObject[] starImages;
    public GameObject halfStarImage;
    public TextMeshProUGUI countText;

    [Header("Items Collected")]
    public GameObject itemPrefab;
    public Transform collectedContainer;
    public Transform essentialsContainer;

    private string Translate(string key)
    {
        if (StringManager.Instance != null)
        {
            string translated = StringManager.Instance.GetText(key);
            if (!string.IsNullOrEmpty(translated) && !translated.Contains("[Missing"))
                return translated;
        }
        return key;
    }

    public void Show(List<string> allCollected)
    {
        // Stars
        int fullStars = (int)FYT_ScoreData.starRating;
        bool hasHalf = (FYT_ScoreData.starRating - fullStars) >= 0.5f;

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(i < fullStars);
        }

        if (halfStarImage != null)
        {
            halfStarImage.SetActive(hasHalf);
        }

        // Count
        countText.text = $"{FYT_ScoreData.essentialCollected} / {FYT_ScoreData.essentialTotal} essential items";

        // Left column — all collected items
        foreach (string item in allCollected)
        {
            GameObject entry = Instantiate(itemPrefab, collectedContainer);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = Translate(item);
        }

        // Right column — essentials checklist
        foreach (string item in FYT_ScoreData.collectedEssentials)
        {
            GameObject entry = Instantiate(itemPrefab, essentialsContainer);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=green>\u2713 {Translate(item)}</color>";
        }

        foreach (string item in FYT_ScoreData.missedEssentials)
        {
            GameObject entry = Instantiate(itemPrefab, essentialsContainer);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>\u2717 {Translate(item)}</color>";
        }
    }
}
