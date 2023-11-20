using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Inventory : MonoBehaviour
{
    private static SD_Inventory instance;
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    
    public static SD_Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SD_Inventory>();
            }
            return instance;
        }
    }
    public void AddItem(GameObject item)
    {
        items.Add(item);
    }
    public void RemoveItem(GameObject item)
    {
        items.Remove(item);
    }
    public bool CheckItem(GameObject item)
    {
        if(items.Contains(item) == true)
        {
            return true;
        }
        else{
            return false;
        }
    }

}
