using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public InventoryUI owner;
    public TextMeshProUGUI categoryText;
    public HousePartType category;
    [SerializeField]Button button;
    public Image bg;

    // Start is called before the first frame update

    void Start()
    {
        button = GetComponent<Button>();
        bg = GetComponent<Image>();
        
    }

    public void InitCategoryButton(InventoryUI owner, HousePartType type)
    {
        this.owner = owner;
        category = type;
        if (StringManager.Instance != null)
        {
            categoryText.text = StringManager.Instance.GetText(type.ToString());
        }
        else
        {
            categoryText.text = type.ToString();
        }
        //categoryText.text = type.ToString();
        button.onClick.AddListener(() => { 
            
            OnButtonSelected();
        });
    }

    public void SetBG(bool state)
    {
        bg.enabled = state;
    }

    void OnButtonSelected()
    {
        if (HH_GameManager.Instance.isTutorial) return;
        if (owner.currentCategory == this) return;
        owner.UpdateInventoryUI(category);
        //UpdateCtegoryButton();
        //foreach (var btn in owner.categories)
        //{
        //    if(btn != this)
        //    {
        //        btn.bg.enabled = false;
        //    }
        //}
    }

    private void OnDisable()
    {
        bg.enabled = false;
    }

    public void UpdateCtegoryButton()
    {
        var temp = owner.currentCategory;
        owner.currentCategory = this;
        owner.previousCategory = temp;
        owner.previousCategory.SetBG(false);
        SetBG(true);
    }
}
