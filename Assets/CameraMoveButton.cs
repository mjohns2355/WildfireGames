using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMoveButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public enum Direction { Left, Right, Up, Down, ZoomIn, ZoomOut };
    public Direction direction;
    Vector2 moveDirection = Vector2.zero;
    bool isZooming = false;
    float zoomAxis;
    // Start is called before the first frame update
    void Start()
    {
        switch (direction)
        {
            case Direction.Left:
                moveDirection = Vector2.left;
                break;
            case Direction.Right:
                moveDirection = Vector2.right;
                break;
            case Direction.Up:
                moveDirection = Vector2.up;
                break;
            case Direction.Down:
                moveDirection = Vector2.down;
                break;
            case Direction.ZoomIn:
                moveDirection = Vector2.zero;
                isZooming = true;
                zoomAxis = 0.1f;
                break;
            case Direction.ZoomOut:
                zoomAxis = -0.1f;
                moveDirection = Vector2.zero;
                isZooming = true;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 localMoveDirection = Camera.main.transform.TransformDirection(moveDirection);
        GameManager.Instance.inputManager.checkKeyboard = false;
        GameManager.Instance.inputManager.cameraMovementVector = new Vector2(localMoveDirection.x, localMoveDirection.z);
        if(isZooming)
        {
            GameManager.Instance.inputManager.cameraZoomAxis = zoomAxis;
        }
        Debug.Log(GameManager.Instance.inputManager.checkKeyboard);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.inputManager.checkKeyboard = true;
        GameManager.Instance.inputManager.cameraMovementVector = Vector2.zero;
        if (isZooming)
        {
            GameManager.Instance.inputManager.cameraZoomAxis = 0f;
        }
        Debug.Log(GameManager.Instance.inputManager.checkKeyboard);
    }
        
}

