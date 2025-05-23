using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Buffers;
public class OptionButton : MonoBehaviour
{
    //public bool isLocked = false;
    //public bool isGoodOption {  get; private set; }
    //public bool needConfirmation;
    public StructureContextMenu owner;
    public Button button;

    [SerializeField] Button learnMoreButton;
    [SerializeField] TextMeshProUGUI optionText;
    [SerializeField] Image checkMark;
    [SerializeField] Sprite circleCheck, circleBlank, squareCheck, squareBlank;
    bool isMultipleChoice = false;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        //button.interactable = !isLocked;
        //optionText.gameObject.SetActive(!isLocked);
        //learnMoreButton.gameObject.SetActive(isLocked);
        
    }

    public void ToggleOptionSelectState(bool state)
    {
        if(state == true)
        {
            checkMark.sprite = isMultipleChoice? squareCheck : circleCheck;
        }
        else
        {
            checkMark.sprite = isMultipleChoice ? squareBlank : circleBlank;
        }
    }

    public string GetOptionContent()
    {
        return optionText.text;
    }
    public void InitOptionButton(StructureContextMenu owner, string buttonText)
    {
        this.owner = owner;
        optionText.text = buttonText;
        //needConfirmation = IsGoodOption(buttonText);
        var house = (HouseStructure)(owner.owner);
        isMultipleChoice = house.houseInfo.allowMultipleChoices;

        checkMark.sprite = isMultipleChoice? squareBlank: circleBlank;
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            //Debug.Log("Option: " + optionText.text + " is clicked");
            //owner.explaination.text = FindOptionExplaination(house);
            owner.OnOptionButtonClicked(this);
            //owner.onOptionSelected.Invoke(this);
            //owner.OnClickGoodOptionButton(this);

        });

        learnMoreButton.onClick.RemoveAllListeners();

        learnMoreButton.onClick.AddListener(() =>
        {

            //Debug.Log("Open Learn More Panel");
            owner.learnMorePopup.gameObject.SetActive(true);
            owner.learnMorePopup.ShowLearnMorePopup(house.houseInfo, buttonText);
            //LearnMorePanel learnMorePanel = ATC_UIController.Instance.learnMorePanel.GetComponent<LearnMorePanel>();
            //ATC_UIController.Instance.PushPanel(learnMorePanel.gameObject);
            //learnMorePanel.OnDetailedPageEnable(house.HouseType, buttonText);
        });
    }

    public void InitIncentiveOptions(string text, FC_IncentivePage owner)
    {
        optionText.text = text;
        // Remove existing listeners and add new
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => owner.OnIncentiveOptionClicked(this));
    }
    public void SetOptionButtonText(string text)
    {
        optionText.text = text;
    }

    public string FindOptionExplaination(HouseStructure house)
    {
        var choice = house.houseInfo.ReturnChoiceByName(optionText.text).choice;

        return choice == null ? null : choice.choiceDetail;
    }

    bool IsGoodOption(string optionName)
    {
        var house = (HouseStructure)(owner.owner);
        var choice = house.houseInfo.ReturnChoiceByName(optionName).choice;

        return choice != null && !choice.isNormal;
    }

}
