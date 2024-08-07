using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    
    public StructureType structureType;
    public StructureContextMenu menu;
    public Transform menuSpawnPos;
    public Outline outline;
    public int height = 1;
    public int width = 1;
    public bool isBigStructure { get {  return width> 1 ||  height> 1; } }
    // people, car, pet
    public Dictionary<string,string> structureInfoDict = new Dictionary<string,string>();
    //[SerializeField]
    //float menuOffset = 5f;

    virtual public void Awake()
    {
        //menu.closeButton.onClick.AddListener(StopSturctureClick);
        //menu.owner = this;
    }

    virtual public void OnStructureClick()
    {
        menu.gameObject.SetActive(true);
        //menu.UpdateText(structureInfoDict);
        outline.enabled = true;
    }

    virtual public void StopSturctureClick()
    {
        HideUI();

    }

    public void HideUI()
    {
        menu.gameObject.SetActive(false);
        outline.enabled = false;
    }


}
