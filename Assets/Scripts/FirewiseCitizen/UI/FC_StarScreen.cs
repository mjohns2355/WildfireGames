using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FC_StarScreen : MonoBehaviour
{
    public GameObject starsContainer, buttonsContainer;
    public Sprite greyStar, yellowStar;
    public Button restart, nextLevel, mainMenu;

    const int HOUSE_PENALTY = 50;
    const int CAR_PENALTY = 50;

    List<GameObject> stars = new List<GameObject>();
    
    // Start is called before the first frame update
    void OnEnable()
    {
        //Debug.Log("Star Container Count: " + starsContainer.transform.childCount);
        for (int i = 0; i< starsContainer.transform.childCount; i++)
        {
            stars.Add(starsContainer.transform.GetChild(i).gameObject);
        }

        restart.onClick.AddListener(() => { GameManager.Instance.ResetGame(); });
        nextLevel.onClick.AddListener(() => { GameManager.Instance.NextLevel(); });
        mainMenu.onClick.AddListener(() => { GameManager.Instance.BackToMainMenu(); });
    }

    private void OnDisable()
    {
        stars.Clear();
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
        if (ratio >= 0.7f) return 3;
        if (ratio >= 0.5f) return 2;
        if (ratio >= 0.2f) return 1;
        return 0;
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
    public void ShowStars()
    {
        int starsCount = CalculateStars();
        Debug.Log("Start Counts " + starsCount);
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < starsCount)
            {
                stars[i].GetComponent<Image>().sprite = yellowStar;
            }
            else
            {
                stars[i].GetComponent<Image>().sprite = greyStar;
            }
        }
    }
}
