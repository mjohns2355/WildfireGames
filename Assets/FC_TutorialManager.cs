using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FC_TutorialManager : MonoBehaviour
{
    public Button titleCard;
    public GameObject bottomDialogBox;
    public TextMeshProUGUI bottomDialogText;
    // Start is called before the first frame update
    void Start()
    {
        titleCard.onClick.AddListener(() =>
        {
            titleCard.gameObject.SetActive(false);
            StartTutorial();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartTutorial()
    {
        bottomDialogBox.SetActive(true);
        UpdateBottomDialog("Welcome to Firewise Citizens! Tap on the Fire Station to Begin");
    }
    void UpdateBottomDialog(string text)
    {
        bottomDialogText.text = text;
    }
}
