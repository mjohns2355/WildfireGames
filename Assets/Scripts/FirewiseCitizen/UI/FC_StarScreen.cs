using System;
using System.Collections;
using System.Collections.Generic;
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
    }



    private void OnDisable()
    {
        restart.onClick.RemoveAllListeners();
        nextLevel.onClick.RemoveAllListeners();
        mainMenu.onClick.RemoveAllListeners();
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
        return Mathf.Clamp(3f - Mathf.Floor(percent * 4f) * 0.5f, 0f, 3f);
    }
    
    public void CalculateHouseProtectedScore()
    {
        int totalHouses = GameManager.Instance.totalHouses;
        int housesDestroyed = GameManager.Instance.housesDestroyed;
        float percentBurned = (float)housesDestroyed / totalHouses;
        ShowStars(CalculateStarRating(percentBurned),houseProtectedStars);
    }
    
    public void CalculateInjuriesProventedScore()
    {
        int totalCars = GameManager.Instance.totalCars;
        int carsNotEvacuated = GameManager.Instance.carsNotEvacuated;
        float percentInjured = (float)carsNotEvacuated / totalCars;
        ShowStars(CalculateStarRating(percentInjured), injuriesPreventedStars);
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
