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
    virtual public void OnStructureClick()
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
}
