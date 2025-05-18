using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Structure : MonoBehaviour
{
    
    public StructureType structureType;
    public StructureContextMenu contextMenu;
    public Transform menuSpawnPos;
    public Vector3Int roadPosition;
    public int height = 1;
    public int width = 1;
    
    public bool IsBigStructure { get {  return width> 1 ||  height> 1; } }
    // people, car, pet
    public Dictionary<string,string> structureInfoDict = new Dictionary<string,string>();
    Outline outline;
    public virtual void Start()
    {
        GameManager.Instance.inputManager.OnStructureClicked += (structure) =>
        {
            if (structure == this)
            {
                OnStructureClick();
            }
        };

        StartCoroutine(DelayedRoadPosition());
        
    }
    public virtual void OnStructureClick()
    {
        //contextMenu.gameObject.SetActive(true);
        ////menu.UpdateText(structureInfoDict);
        //outline.enabled = true;
        
    }

    virtual public void StopSturctureClick()
    {

        HideUI();

    }

    public void HideUI()
    {
        contextMenu.gameObject.SetActive(false);
        //outline.enabled = true;
    }

    IEnumerator DelayedRoadPosition()
    {
        yield return new WaitForSeconds(0.1f);
        roadPosition = GetComponent<ATC_StructureModel>().RoadPosition;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void SetOutline(bool isActive)
    {
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = isActive;
    }

    // make driveway face to the nearest road
    public void ModifyStructureRotation()
    {
        Vector3Int delta = roadPosition - Vector3Int.RoundToInt(transform.position);
        //Debug.Log($"Road Position: {roadPosition}, Structure Position: {Vector3Int.RoundToInt(transform.position)}, Delta: {delta}");
        // Step 1: Pick dominant direction
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            // Horizontal
            if (delta.x > 0)
            {
                //Debug.Log("Face East");
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                //Debug.Log("Face West");
                transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
        else
        {
            // Vertical
            if (delta.z > 0)
            {
                //Debug.Log("Face North");
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                //Debug.Log("Face South");
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
