using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelSlot : MonoBehaviour, IPointerClickHandler
{
    public Image bg;
    public Image icon;
    public TextMeshProUGUI partName;

    public HousePart partInfo;
    public float distanceFromCamera = 10f;

    GameObject houseObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ObjFollowCursor();
    }

    private void ObjFollowCursor()
    {
        if (houseObj == null) return;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        houseObj.transform.position = worldPosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
        {
            var hitObj = hit.collider.gameObject;

            if (hitObj.TryGetComponent(out BaseHousePartObject housePartObj))
            {
                if (housePartObj.HousePartType == HousePartType.Roof)
                {
                    //Debug.Log(housePartObj.housePart.materialType + " " + housePartObj.housePart.housePartType);
                }
            }

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Color color = bg.color;
        color.a = 0.5f;
        bg.color = color;

        houseObj = CreateHouseObject(partInfo).gameObject;
    }

    BaseHousePartObject CreateHouseObject(HousePart part)
    {
        var obj = new GameObject(part.name);
        var houseObj = obj.AddComponent<BaseHousePartObject>();
        houseObj.isOnCursor = true;
        //houseObj.housePart = part;
        houseObj.meshRenderer = Instantiate(part.mesh,houseObj.transform).GetComponent<MeshRenderer>();
        return houseObj;    
    }

}
