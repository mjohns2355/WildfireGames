using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FYT_ResultsScreen : MonoBehaviour
{
    [Header("Middle Column")]
    public List<Image> starImages;
    public Sprite emptyStar, halfStar, fullStar;
    public TextMeshProUGUI countText;

    [Header("Items Collected")]
    public GameObject itemPrefab;
    public Transform collectedContainer;
    public Transform essentialsContainer;

    //Checklist Assets
    public Sprite greenContainer, redContainer;

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
        // Stars taken from the FR + FF code base
        UpdateStarVisuals(FYT_ScoreData.starRating, starImages);
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

        // Count
        countText.text = $"{FYT_ScoreData.essentialCollected} / {FYT_ScoreData.essentialTotal} essential items";

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

        // Right column — essentials checklist
        foreach (string item in FYT_ScoreData.collectedEssentials)
        {
            GameObject entry = Instantiate(itemPrefab, essentialsContainer);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = Translate(item);
            entry.GetComponent<Image>().sprite = greenContainer;
            entry.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.12f, 0.35f, 0.12f);
            //entry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=green>\u2713 {Translate(item)}</color>";
        }

        foreach (string item in FYT_ScoreData.missedEssentials)
        {
            GameObject entry = Instantiate(itemPrefab, essentialsContainer);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = Translate(item);
            entry.GetComponent<Image>().sprite = redContainer;
            entry.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.45f, 0.22f, 0.1f);
            //entry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>\u2717 {Translate(item)}</color>";
        }
    }

    public void UpdateStarVisuals(float stars, List<Image> starImages)
    {
        int fullStars = Mathf.FloorToInt(stars);
        bool hasHalfStar = (stars - fullStars) >= 0.5f;

        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < fullStars)
                starImages[i].sprite = fullStar;
            else if (i == fullStars && hasHalfStar)
                starImages[i].sprite = halfStar;
            else
                starImages[i].sprite = emptyStar;
        }
    }
}
