using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    
    public StructureType structureType;
    public StructureContextMenu contextMenu;
    public Transform camFocusPos;
    public Transform menuSpawnPos;
    public Vector3Int roadPosition;
    public Vector3 roadDirection;
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

    public void CheckRoadDirection()
    {
       
        if (Mathf.Abs(roadPosition.x - transform.position.x) > Mathf.Abs(roadPosition.z - transform.position.z))
        {
            roadDirection = Vector3.right; // Horizontal road
        }
        else
        {
            roadDirection = Vector3.forward; // Vertical road
        }
    }
}
