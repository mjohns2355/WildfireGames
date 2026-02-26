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
    public TextMeshProUGUI collectedListText;

    [Header("Essentials Checklist")]
    public TextMeshProUGUI essentialsListText;

    [Header("Panel")]
    public GameObject resultsPanel;

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

        // Left column — all collected items (translated)
        List<string> translatedCollected = new List<string>();
        foreach (string item in allCollected)
        {
            translatedCollected.Add(Translate(item));
        }
        collectedListText.text = string.Join("\n", translatedCollected);

        // Right column — essentials checklist (translated)
        List<string> lines = new List<string>();

        foreach (string item in FYT_ScoreData.collectedEssentials)
        {
            lines.Add($"<color=green>\u2713 {Translate(item)}</color>");
        }

        foreach (string item in FYT_ScoreData.missedEssentials)
        {
            lines.Add($"<color=red>\u2717 {Translate(item)}</color>");
        }

        essentialsListText.text = string.Join("\n", lines);

        resultsPanel.SetActive(true);
    }
}
