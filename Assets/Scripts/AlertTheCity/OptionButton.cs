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
        owner.explaination.text = FindOptionExplaination((HouseStructure)(owner.owner));
        button.onClick.AddListener(() =>
        {
            owner.onOptionSelected.Invoke(this);
            owner.OnClickGoodOptionButton(isGoodOption);
            if (isGoodOption)
            {
                isGoodOption = false;
            }
        });
    }
    public void SetOptionButtonText(string text)
    {
        optionText.text = text;
    }

    public string FindOptionExplaination(HouseStructure house)
    {
        foreach (var choice in house.houseInfo.lockedChoices)
        {
            if (choice.choiceName == optionText.text)
            {
                isGoodOption = true;
                return house.houseInfo.lockedOptionDetail;
            }
        }
        //if (house.info.lockedOptions.ContainsKey(optionText.text))
        //{
        //    isGoodOption = true;
        //    return house.info.lockedOptions[optionText.text];
        //}


        return null;
    }


}
