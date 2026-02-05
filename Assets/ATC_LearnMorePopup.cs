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
        title.text = StringManager.Instance.GetText(optionText);
        var result = info.ReturnChoiceByName(optionText);
        
        if (result.choice != null)
        {
            if (optionText == "planAheadText")
            {
                detail.text = StringManager.Instance.GetText("planAheadDetailText");
            }
            else
            {
                detail.text = StringManager.Instance.GetText(result.choice.choiceDetail);
            }
        }

    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
