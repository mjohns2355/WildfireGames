using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    public enum StructureType { House, Shelter}
    public int memberNum;
    public int petNum;
    public int carNum;
    public bool hasElder;
    public bool hasPet;
    public StructureType structureType;
    public GameObject contextMenu;
    [SerializeField]
    float menuOffset = 5f;
    private void Start()
    {
       
    }
    public void OnStructureClick()
    {
        contextMenu.SetActive(true);

        // Offset position above object bbox (in world space)
        float offsetPosY = transform.position.y + menuOffset;

        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
        var canvasRect = GameManager.Instance.uiController.canvas.GetComponent<RectTransform>();
        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
        contextMenu.transform.SetParent(GameManager.Instance.uiController.canvas.transform);
        // Set
        contextMenu.transform.localPosition = canvasPos;
    }
}
