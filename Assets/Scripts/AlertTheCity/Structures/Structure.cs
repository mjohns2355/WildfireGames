using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    public enum StructureType { House, Shelter}
    public StructureType structureType;
    public StructureContextMenu menu;
    // people, car, pet
    protected Dictionary<string,int> structureInfoDict = new Dictionary<string,int>();
    [SerializeField]
    float menuOffset = 5f;


    virtual public void OnStructureClick()
    {
        menu.gameObject.SetActive(true);
        menu.UpdateText(structureInfoDict);
    }

    
}
