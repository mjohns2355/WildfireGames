using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    
    public StructureType structureType;
    public StructureContextMenu contextMenu;
    public Transform menuSpawnPos;
    public Outline outline;
    public int height = 1;
    public int width = 1;
    public bool IsBigStructure { get {  return width> 1 ||  height> 1; } }
    // people, car, pet
    public Dictionary<string,string> structureInfoDict = new Dictionary<string,string>();
    
    virtual public void OnStructureClick()
    {
        contextMenu.gameObject.SetActive(true);
        //menu.UpdateText(structureInfoDict);
        outline.enabled = true;
    }

    virtual public void StopSturctureClick()
    {
        HideUI();

    }

    public void HideUI()
    {
        contextMenu.gameObject.SetActive(false);
        outline.enabled = false;
    }


}
