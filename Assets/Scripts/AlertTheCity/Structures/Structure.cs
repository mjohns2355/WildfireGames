using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    public enum StructureType { House, Shelter}
    public StructureType structureType;
    public StructureContextMenu menu;
    public Outline outline;
// people, car, pet
    public Dictionary<string,string> structureInfoDict = new Dictionary<string,string>();
    //[SerializeField]
    //float menuOffset = 5f;

    virtual public void Awake()
    {
        menu.closeButton.onClick.AddListener(StopSturctureClick);
        menu.owner = this;
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
