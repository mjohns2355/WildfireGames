using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OptionButton : MonoBehaviour
{
    public bool isLocked = false;
    public bool isGoodOption = false;
    public StructureContextMenu owner;
    [SerializeField] Button button;
    [SerializeField]TextMeshProUGUI optionText;

    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        button.interactable = !isLocked;
        optionText.gameObject.SetActive(!isLocked);
    }

    public string GetOptionContent()
    {
        return optionText.text;
    }
    public void InitOptionButton(StructureContextMenu owner, string buttonText)
    {
        this.owner = owner;
        optionText.text = buttonText;
        isGoodOption = IsGoodOption(buttonText);
        button.onClick.AddListener(() =>
        {
            //Debug.Log("Option: " + optionText.text + " is clicked");
            owner.explaination.text = FindOptionExplaination((HouseStructure)(owner.owner));
            owner.OnOptionButtonClicked(this);
            //owner.onOptionSelected.Invoke(this);
            //owner.OnClickGoodOptionButton(this);

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
        var choice = house.houseInfo.ReturnChoiceByName(optionText.text);

        return choice == null ? null : choice.choiceDetail;
    }

    bool IsGoodOption(string optionName)
    {
        var house = (HouseStructure)(owner.owner);
        var choice = house.houseInfo.ReturnChoiceByName(optionName, true);

        return choice != null;
    }

}
