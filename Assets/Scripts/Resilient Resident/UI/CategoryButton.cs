using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public InventoryUI owner;
    public TextMeshProUGUI categoryText;
    public HousePartType category;
    Button button;
    // Start is called before the first frame update
    private void Awake()
    {
       
    }
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { owner.UpdateOwnedParts(category); });
    }

    public void InitCategoryButton(InventoryUI owner, HousePartType type)
    {
        this.owner = owner;
        category = type;
        categoryText.text = type.ToString();
    }
}
