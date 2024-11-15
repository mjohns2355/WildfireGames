using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;
    [Range(0f, 1f)]
    [SerializeField] float cameraZoomSpeed;
    [SerializeField] float lerpSpeed;
    [SerializeField] private float maxFOV;
    [SerializeField] private float minFOV;
    [SerializeField] private float defaultFOV;
    public float focusDistance = 5f;
    public Vector3 camPosOffset = Vector3.zero;
    private Vector3 targetPosition;
    private bool isFocusing = false;
    float FOV;
    Vector3 camPos;
    private Vector3 camStartPos;
    private Quaternion camStartRotation;
    private void Start()
    {
        camStartPos = transform.position;
        camStartRotation = transform.rotation;
        gameCamera = GetComponent<Camera>();
        gameCamera.fieldOfView = defaultFOV;
        FOV = gameCamera.fieldOfView;
        camPos = gameCamera.transform.position;

        //GameManager.Instance.inputManager.OnMouseHold += DragToMoveCamera;
        //GameManager.Instance.inputManager.OnMouseUp += ResetMousePosition;
    }

    private void Update()
    {
        if (isFocusing)
        {

            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraMovementSpeed * Time.deltaTime);


            gameCamera.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 30, cameraZoomSpeed * Time.deltaTime);


            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isFocusing = false;

                //Debug.Log("Stop focusing");
                //HH_GameManager.Instance.inputManager.OnHouseSelected -= MoveToHouse;
            }
        }
    }
    public void MoveCamera(Vector3 inputVector)
    {
        //Debug.Log("Input Vector: " + inputVector);
        //var movementVector = Quaternion.Euler(0, 30, 0) * inputVector;
        //gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        camPos += inputVector * Time.deltaTime * cameraMovementSpeed;

        float clampedX = Mathf.Clamp(camPos.x, 40f, 60f);
        float clampedZ = Mathf.Clamp(camPos.z, -35f, 5f);

        camPos = new Vector3(clampedX, camPos.y, clampedZ);

        gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, camPos, Time.deltaTime * lerpSpeed);
    }

    public void ZoomCamera(float mouseAxis)
    {

        FOV += mouseAxis * -1 * cameraZoomSpeed;
        FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
        gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, FOV, Time.deltaTime * lerpSpeed);
    }

    public void ZoomCamera()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float touchDeltaMag = (touch1.position - touch2.position).magnitude;

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        FOV -= deltaMagnitudeDiff * Time.deltaTime;
        FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
        gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, FOV, Time.deltaTime * lerpSpeed);
    }

    public void MoveToHouse(Transform targetHouse)
    {
        //Debug.Log($"Move to house {targetHouse.transform.position}");
        GameManager.Instance.canControlCam = false;
        targetPosition = targetHouse.position + camPosOffset - targetHouse.forward * focusDistance;
        isFocusing = true;
    }

    public void ResetCam()
    {
        transform.SetPositionAndRotation(camStartPos, camStartRotation);
        GameManager.Instance.canControlCam = true;
    }
}
