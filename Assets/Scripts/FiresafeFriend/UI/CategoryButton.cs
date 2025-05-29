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
        categoryText.text = type.ToString();
        button.onClick.AddListener(() => { 
            
            OnButtonSelected();
        });
    }

    void OnButtonSelected()
    {
        owner.UpdateInventoryUI(category);

        foreach (var btn in owner.categories)
        {
            if(btn != this)
            {
                btn.bg.enabled = false;
            }
        }
        bg.enabled = true;
    }

    private void OnDisable()
    {
        bg.enabled = false;
    }
}
