using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ATC_InputManager : MonoBehaviour
{
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    public Action<int> OnMouseScroll;
    public Action<Structure> OnStructureClicked;
    public float cameraZoomAxis;
    public Vector2 cameraMovementVector;
    public bool checkKeyboard;
	[SerializeField] Camera mainCamera;

	public LayerMask groundMask;
    public LayerMask structureMask;
    public LayerMask uiMask;
    [SerializeField] private LayerMask targetLayer;

    private Vector3 lastTouchPosition;
    private bool isDragging;
    public bool isKeyboard = false;


    private void Start()
    {
        targetLayer = structureMask;
        checkKeyboard = true;
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
        CheckArrowInput();
        if (Input.touchSupported && Input.touchCount > 0)
            CheckDragInput();    // touch input
        else
            CheckMouseDrag();    // click & drag


    }

    private Vector3Int? RaycastGround()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer)) {
            Vector3Int positionInt = Vector3Int.RoundToInt(hit.point);
            return positionInt;

        }

        return null;
    }

    private Structure? RaycastStructure()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, structureMask))
        {
            var structure = hit.collider.gameObject.GetComponent<Structure>();
            return structure;
        }
        return null;
    }
    private void CheckArrowInput()
    {

        if (!checkKeyboard) return;
        isKeyboard = true;
        cameraMovementVector = Vector2.zero;
        cameraZoomAxis = Input.GetAxis("Mouse ScrollWheel") * 10f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 localMoveDirection = Camera.main.transform.TransformDirection(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            cameraMovementVector = new Vector2(localMoveDirection.x, localMoveDirection.z);
            checkKeyboard = true;
        }

    }
    private void CheckMouseDrag()
    {
        if (!checkKeyboard) return;
        isKeyboard = false;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            isDragging = true;
            checkKeyboard = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");
            Vector3 localMoveDir = mainCamera.transform.TransformDirection(new Vector3(dx, 0f, dy));
            cameraMovementVector = new Vector2(-localMoveDir.x, -localMoveDir.z) * 5f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
    private void CheckDragInput()
    {
        if (Input.touchCount == 1 && !IsPointerOverUI())
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
                checkKeyboard = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 touchDelta = (Vector3)touch.position - lastTouchPosition;
                Vector3 localMoveDirection = Camera.main.transform.TransformDirection(touchDelta);
                cameraMovementVector = new Vector2(-localMoveDirection.x,-localMoveDirection.z) * 5f * Time.deltaTime;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
            isKeyboard = false;
        }
    }

    private void CheckClickHoldEvent()
    {
        if(Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            var position  = RaycastGround();
            if(position != null)
            {
                OnMouseHold?.Invoke(position.Value);
            }

        }

    }

    private void CheckClickUpEvent()
    {
        if (Input.GetMouseButtonUp(0) && !IsPointerOverUI())
        {
            var position = RaycastGround();
            if (position != null)
            {
                OnMouseUp?.Invoke();
            }
        }
    }

    private void CheckClickDownEvent()
    {
        
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            var position = RaycastGround();
            if (position != null)
            {
                OnMouseClick?.Invoke(position.Value);

            }
            var structure = RaycastStructure();
            if(structure != null)
            {
                OnStructureClicked?.Invoke(structure);
            }
        }
    }
    private bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        else
            return EventSystem.current.IsPointerOverGameObject();

    }
    public void OnConstructionMode(bool state)
    {
        if (state == false)
        {
            targetLayer = structureMask;
        }
        else
        {
            targetLayer = groundMask;
        }
    }
}
