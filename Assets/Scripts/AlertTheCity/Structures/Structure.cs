using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    public enum StructureType { House, Shelter}
    public int pplNum;
    public int petNum;
    public int carNum;
    public bool hasElder;
    public bool hasPet;
    public StructureType structureType;
    public StructureContextMenu menu;
    // people, car, pet
    Dictionary<string,int> structureInfoDict = new Dictionary<string,int>();
    [SerializeField]
    float menuOffset = 5f;
    private void Awake()
    {
        structureInfoDict.Add("People", pplNum);
        structureInfoDict.Add("Car(s)", carNum);
        structureInfoDict.Add("Pet(s)", petNum);
    }

    public void OnStructureClick()
    {
        menu.gameObject.SetActive(true);
        menu.UpdateText(structureInfoDict);
    }

    
}
