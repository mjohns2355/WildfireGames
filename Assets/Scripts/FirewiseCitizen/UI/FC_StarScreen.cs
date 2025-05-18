using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class FC_StarScreen : MonoBehaviour
{
    public GameObject starsContainer, buttonsContainer;
    public Sprite emptyStar, halfStar, fullStar;
    public Button restart, nextLevel, mainMenu,restartFromBeginning;

    const int HOUSE_PENALTY = 50;
    const int CAR_PENALTY = 50;

    [SerializeField] List<Image> houseProtectedStars = new List<Image>();
    [SerializeField] List<Image> injuriesPreventedStars = new List<Image>();
    [SerializeField] List<Image> converstationQualityStars = new List<Image>();

    // Start is called before the first frame update
    void OnEnable()
    {

        restart.onClick.AddListener(() => { GameManager.Instance.ResetGame(); });
        nextLevel.onClick.AddListener(() => { GameManager.Instance.NextLevel(); });
        mainMenu.onClick.AddListener(() => { GameManager.Instance.BackToMainMenu(); });
        restartFromBeginning.onClick.AddListener(() => { GameManager.Instance.RestartGameFromTutorial(); });

        nextLevel.gameObject.SetActive(!GameManager.Instance.IsLastLevel);
        restartFromBeginning.gameObject.SetActive(GameManager.Instance.IsLastLevel);

        CalculateHouseProtectedScore();
        CalculateInjuriesProventedScore();
        CalculateConvoQualityScore();
    }



    private void OnDisable()
    {
        restart.onClick.RemoveAllListeners();
        nextLevel.onClick.RemoveAllListeners();
        mainMenu.onClick.RemoveAllListeners();

        for (int i = 0; i<3;i++)
        {
            houseProtectedStars[i].sprite = emptyStar;
            injuriesPreventedStars[i].sprite = emptyStar;
            converstationQualityStars[i].sprite = emptyStar;
        }
    }

    public int CalculateStars()
    {
        int maxPenalty = CalculateMaxPenalty();
        int damage = CalculateDamageScore();

        float ratio = 1f - Mathf.Clamp01((float)damage / maxPenalty);
        Debug.Log("Damage: " + damage + " ,Max Penalty: " + maxPenalty + " ,Ratio: " + ratio);
        if (ratio >= 0.8f) return 3;
        if (ratio >= 0.6f) return 2;
        if (ratio >= 0.4f) return 1;
        return 0;
    }

    float CalculateStarRating(float percent)
    {
        //Debug.Log($"Percent: {percent}");
        //Debug.Log($"Star Rating: {Mathf.Clamp(3f - Mathf.Floor(percent * 4f) * 0.5f, 0f, 3f)}");
        return Mathf.Clamp(3f - Mathf.Ceil(percent * 4f) * 0.5f, 0f, 3f);
    }
    
    public void CalculateHouseProtectedScore()
    {
        int totalHouses = GameManager.Instance.totalHouses;
        int housesDestroyed = GameManager.Instance.housesDestroyed;
        var wuiHouseCount = GameManager.Instance.structureManager.houseTypeDict[HouseType.wui].Count + 1;
        int houseNotHomeHardening = wuiHouseCount - GameManager.Instance.houseHasHomeHardening;
        int effectiveDestroyed = housesDestroyed + houseNotHomeHardening;
        effectiveDestroyed = Mathf.Min(effectiveDestroyed, totalHouses);
        float percentBurned = (float)effectiveDestroyed / totalHouses;
        Debug.Log($"Total House:{totalHouses}, House Destroyed: {housesDestroyed}, House Not Home Hardening: {houseNotHomeHardening}, Effective Destroyed: {effectiveDestroyed}, Percent:{percentBurned}");
        ShowStars(CalculateStarRating(percentBurned),houseProtectedStars);
    }
    
    public void CalculateInjuriesProventedScore()
    {
        int totalCars = GameManager.Instance.totalCars;
        int carsNotEvacuated = totalCars -  GameManager.Instance.carsEvacuated;
        float percentInjured = (float)carsNotEvacuated / totalCars;
        Debug.Log($"Total Cars: {totalCars}, Injuries: {carsNotEvacuated}");
        ShowStars(CalculateStarRating(percentInjured), injuriesPreventedStars);
    }

    public void CalculateConvoQualityScore()
    {
        int rewardStars = 0;
        int totalStars = GameManager.Instance.availableHouseTypes.Count * 3;
        var finalChoices = GameManager.Instance.finalChoices;
        var dialogueFlags = ATC_UIController.Instance.houseDialogManager.dialogFlagsMap;
        var houseTypeInfo = GameManager.Instance.structureManager.houseInfoDict;
        var skippedAll = !ATC_UIController.Instance.contextMenus.Any(menu => menu.isSelected && menu.owner is HouseStructure);

        if (skippedAll)
        {
            ShowStars(0, converstationQualityStars);
            return;
        }
        foreach (var pair in finalChoices)
        {
            var typeInfo = houseTypeInfo[pair.Key];
            var key = pair.Key.ToString();
            var flag = dialogueFlags[key];
            var finalChoice = pair.Value;
            var totalChoices = typeInfo.allChoicesCount;
            var choiceIndex = typeInfo.ReturnChoiceByName(finalChoice.choiceName).index;
            rewardStars += CalculateStars(choiceIndex, totalChoices, flag.Item2);
        }

        int CalculateStars(int choiceIndex, int totalChoicesCount, bool skipped)
        {
            // reversedIndex = 0 for the last (best) option, 1 for second-last, etc.
            int reversedIndex = (totalChoicesCount - 1) - choiceIndex;

            // Map reversedIndex → stars: 0→3, 1→2, 2+→1
            int stars = Mathf.Clamp(3 - reversedIndex, 1, 3);

            // Apply skip penalty
            if (skipped)
                stars = Mathf.Max(1, stars - 1);

            return stars;
        }
        var percent = (float) (totalStars - rewardStars) / totalStars;
        //Debug.Log($"Conversation Quality Score: {rewardStars}/{totalStarts} = {percent}");
        ShowStars(CalculateStarRating(percent),converstationQualityStars);
    }



    public int CalculateDamageScore()
    {
        int damage = 0;
        damage += GameManager.Instance.housesDestroyed * HOUSE_PENALTY;
        damage += GameManager.Instance.carsNotEvacuated * CAR_PENALTY;
        return damage;
    }

    public int CalculateMaxPenalty()
    {
        int penalty = 0;
        penalty += GameManager.Instance.totalHouses * HOUSE_PENALTY;
        penalty += GameManager.Instance.totalCars * CAR_PENALTY;
        return penalty;
    }
    public void ShowStars(float stars, List<Image> starImages)
    {
        int fullStars = Mathf.FloorToInt(stars);
        bool hasHalfStar = stars - fullStars >= 0.5f;

        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < fullStars)
                starImages[i].sprite = fullStar;
            else if (i == fullStars && hasHalfStar)
                starImages[i].sprite = halfStar;
            else
                starImages[i].sprite = emptyStar;
        }
        //int starsCount = CalculateStars();
        //Debug.Log("Start Counts " + starsCount);
        //for (int i = 0; i < stars.Count; i++)
        //{
        //    if (i < starsCount)
        //    {
        //        stars[i].GetComponent<Image>().sprite = yellowStar;
        //    }
        //    else
        //    {
        //        stars[i].GetComponent<Image>().sprite = greyStar;
        //    }
        //}
    }
}
