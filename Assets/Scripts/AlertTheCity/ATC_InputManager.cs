using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ATC_InputManager : MonoBehaviour
{
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    public Action<int> OnMouseScroll;
	private Vector2 cameraMovementVector;
	[SerializeField]
	Camera mainCamera;

	public LayerMask groundMask;
    public LayerMask structureMask;
    public LayerMask uiMask;
    [SerializeField]

    private LayerMask targetLayer;

    public Vector2 CameraMovementVector
    {
		get { return cameraMovementVector; }
	}



    private void Start()
    {
        targetLayer = structureMask;
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
        CheckArrowInput();
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
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
