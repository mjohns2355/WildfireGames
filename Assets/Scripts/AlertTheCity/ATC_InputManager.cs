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
    [SerializeField]

    private LayerMask targetLayer;

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
        cameraMovementVector = Vector2.zero;
        cameraZoomAxis = Input.GetAxis("Mouse ScrollWheel") * 10f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
        {
            Vector3 localMoveDirection = Camera.main.transform.TransformDirection(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            cameraMovementVector = new Vector2(localMoveDirection.x, localMoveDirection.z);
            checkKeyboard = true;
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
