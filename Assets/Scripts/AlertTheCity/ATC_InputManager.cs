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
    public float cameraZoomAxis;
    public Vector2 cameraMovementVector;
    public bool checkKeyboard;
	[SerializeField]
	Camera mainCamera;

	public LayerMask groundMask;
    public LayerMask structureMask;
    public LayerMask uiMask;
    [SerializeField] private LayerMask targetLayer;

    private Vector3 lastTouchPosition;
    private bool isDragging;
    public bool isKeyboard = false;
    //   public Vector2 CameraMovementVector
    //   {
    //	get { return cameraMovementVector; }
    //}



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
#if UNITY_EDITOR
        SimulateTouchWithMouse();  // In the editor, simulate touch input with the mouse
#else
        CheckDragInput();  // On mobile devices, use real touch input
#endif


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

    private void SimulateTouchWithMouse()
    {
        if(!checkKeyboard) return;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            lastTouchPosition = Input.mousePosition;
            isDragging = true;
            checkKeyboard = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 touchDelta = Input.mousePosition - lastTouchPosition;
            cameraMovementVector = new Vector3(-touchDelta.x, -touchDelta.y, 0) * Time.unscaledDeltaTime * 5f;
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = false;
        }
    }
    private void CheckDragInput()
    {
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject())
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
                cameraMovementVector = new Vector3(-touchDelta.x, -touchDelta.y, 0) * Time.unscaledDeltaTime * 5f;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    private void CheckClickHoldEvent()
    {
        if(Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
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
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
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

        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if (position != null)
            {
                OnMouseClick?.Invoke(position.Value);

            }
        }
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
