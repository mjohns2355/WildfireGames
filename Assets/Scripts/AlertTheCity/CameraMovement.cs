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
    public float focusDistance = 10f;
    public Vector3 camPosOffset = Vector3.zero;
    private Vector3 targetPosition;
    private Transform target;
    private bool isFocusing = false;
    float FOV;
    Vector3 camPos;
    private Vector3 camStartPos;
    private Quaternion camStartRotation;
    float smoothTime = 0.1f;
    float velocity = 0.0f;

    float touchDist = 0;
    float lastDist = 0;
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
        //if (isFocusing)
        //{

        //    //transform.position = Vector3.Lerp(transform.position, targetPosition, cameraMovementSpeed * Time.deltaTime);
        //    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraMovementSpeed * Time.deltaTime);
        //    ////transform.LookAt(target.position);
        //    gameCamera.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 30, cameraZoomSpeed * Time.deltaTime);
        //    transform.LookAt(target);

        //    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        //    {
        //        isFocusing = false;

        //        //Debug.Log("Stop focusing");
        //        //HH_GameManager.Instance.inputManager.OnHouseSelected -= MoveToHouse;
        //    }
        //}

        if(isFocusing)
        {
            gameCamera.transform.LookAt(target.position);
            gameCamera.fieldOfView = 5;
            transform.position = targetPosition;
            
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
    {    // Check for desktop input

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began)
            {
                lastDist = Vector2.Distance(touch1.position, touch2.position);
            }

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                float newDist = Vector2.Distance(touch1.position, touch2.position);
                touchDist = lastDist - newDist;
                lastDist = newDist;


                if (Mathf.Abs(touchDist) > 0.01f) // Ignore very small changes
                {
                    float sensitivity = 0.1f;
                    touchDist = Mathf.Clamp(touchDist, -50f, 50f);
                    FOV += touchDist * sensitivity;
                    FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
                    gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
                }
            }


            //float zoomChange = (previousTouchDistance - currentTouchDistance)/Screen.height*100f;


            //FOV -= zoomChange * 10f * Time.unscaledDeltaTime;
            //FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
            ////gameCamera.fieldOfView = FOV;
            //gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
        }
        else
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Use scroll wheel for zooming
           
            if (Mathf.Abs(scrollInput) > 0.01f) // Small threshold to avoid noise
            {
                float adjustedScrollInput = scrollInput * 100f;
                FOV -= adjustedScrollInput * 10f * Time.unscaledDeltaTime; // Scroll forward zooms in, backward zooms out
                FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
                gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
            }
        }

    }

    public void MoveToHouse(Structure targetHouse)
    {

        //Debug.Log($"Move to house {targetHouse.transform.position}");
        GameManager.Instance.canControlCam = false;
        Vector3 roadToHouse = (targetHouse.transform.position - targetHouse.roadPosition).normalized;


        targetPosition = targetHouse.roadPosition - (roadToHouse* 10f);
        targetPosition.y += 5f;

        target = targetHouse.transform;


        isFocusing = true;
    }

    public void ResetCam()
    {
        isFocusing = false;
        transform.SetPositionAndRotation(camStartPos, camStartRotation);
        GameManager.Instance.canControlCam = true;
    }
}
