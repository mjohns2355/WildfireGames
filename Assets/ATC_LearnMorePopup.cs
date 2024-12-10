using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ATC_LearnMorePopup : MonoBehaviour
{
    public TextMeshProUGUI title, detail;
    public Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
    }
    public void ShowLearnMorePopup(HouseTypeInfo info, string optionText)
    {

        title.text = optionText;
        detail.text = info.ReturnChoiceByName(optionText).choice.choiceDetail;

    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
