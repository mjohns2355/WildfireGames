using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class happyHouseItems : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform myRect;
    private Vector3 tempPos;
    public bool isDragging = false;

    void Start()
    {
        myRect = GetComponent<RectTransform>();
        tempPos.z = myRect.position.z;

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            tempPos.x = Input.mousePosition.x;
            tempPos.y = Input.mousePosition.y;
            myRect.position = tempPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Vector3.Distance(Input.mousePosition, myRect.position) < 250)
        {
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

}
