using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OptionButton : MonoBehaviour
{
    public bool isLocked = false;
    //public bool isGoodOption {  get; private set; }
    public bool needConfirmation;
    public StructureContextMenu owner;
    public Button button;
    [SerializeField] Button learnMoreButton;
    [SerializeField] TextMeshProUGUI optionText;
    [SerializeField] Image checkMark;
    [SerializeField] Sprite check, blank;
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
            checkMark.sprite = check;
        }
        else
        {
            checkMark.sprite = blank;
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
        needConfirmation = IsGoodOption(buttonText);
        var house = (HouseStructure)(owner.owner);

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            //Debug.Log("Option: " + optionText.text + " is clicked");
            owner.explaination.text = FindOptionExplaination(house);
            owner.OnOptionButtonClicked(this);
            //owner.onOptionSelected.Invoke(this);
            //owner.OnClickGoodOptionButton(this);

        });

        learnMoreButton.onClick.RemoveAllListeners();

        learnMoreButton.onClick.AddListener(() =>
        {

            //Debug.Log("Open Learn More Panel");
            //LearnMorePanel learnMorePanel = ATC_UIController.Instance.learnMorePanel.GetComponent<LearnMorePanel>();
            //ATC_UIController.Instance.PushPanel(learnMorePanel.gameObject);
            //learnMorePanel.OnDetailedPageEnable(house.HouseType, buttonText);
        });
    }
    public void SetOptionButtonText(string text)
    {
        optionText.text = text;
    }

    public void OnClick()
    {
        
        button.onClick.Invoke();
    }
    public string FindOptionExplaination(HouseStructure house)
    {
        var choice = house.houseInfo.ReturnChoiceByName(optionText.text).choice;

        return choice == null ? null : choice.choiceDetail;
    }

    bool IsGoodOption(string optionName)
    {
        var house = (HouseStructure)(owner.owner);
        var choice = house.houseInfo.ReturnChoiceByName(optionName, true).choice;

        return choice != null && !choice.isNormal;
    }

}
