using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OptionButton : MonoBehaviour
{
    public bool isLocked = false;
    public StructureContextMenu owner;
    public Button button;
    [SerializeField]TextMeshProUGUI optionText;
    public bool isGoodOption = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        button.interactable = !isLocked;
        optionText.gameObject.SetActive(!isLocked);
    }

    public void InitOptionButton(StructureContextMenu owner, string buttonText)
    {
        this.owner = owner;
        optionText.text = buttonText;
        owner.explaination.text = FindOptionExplaination((HouseStructure)(owner.owner));
    }
    public void SetOptionButtonText(string text)
    {
        optionText.text = text;
    }

    public string FindOptionExplaination(HouseStructure house)
    {
        if (house.info.lockedOptions.ContainsKey(optionText.text))
        {
            isGoodOption = true;
            return house.info.lockedOptions[optionText.text];
        }


        return null;
    }



}
