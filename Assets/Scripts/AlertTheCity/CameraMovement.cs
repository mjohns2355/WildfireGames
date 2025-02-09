using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;
    [Range(0f, 10f)]
    [SerializeField] float cameraZoomSpeed,cameraZoomToHouseSpeed;
    [SerializeField] float lerpSpeed;
    [SerializeField] private float maxFOV;
    [SerializeField] private float minFOV;
    [SerializeField] private float defaultFOV;
    public float focusDistance;
    public Vector3 camPosOffset = Vector3.zero;
    private Vector3 targetPosition;
    private Transform target;
    private bool isFocusing = false;
    float FOV;
    Vector3 camPos;
    private Vector3 camStartPos;
    private Quaternion camStartRotation;
    private float camStartFOV,targetFOV;
    float smoothTime = 0.1f;
    float velocity = 0.0f;
    [SerializeField] private GameObject lastHit;
    float touchDist = 0;
    float lastDist = 0;
    public LayerMask ignoreLayerMask;
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
        if(isFocusing)
        {
            
            //gameCamera.fieldOfView = 5;
            //transform.position = targetPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraZoomToHouseSpeed * Time.deltaTime);
            gameCamera.transform.LookAt(target.position);

            gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, targetFOV, cameraZoomToHouseSpeed * Time.deltaTime);

            RaycastHit hit;

            if (Physics.Raycast(transform.position,transform.forward, out hit, Mathf.Infinity, ignoreLayerMask, QueryTriggerInteraction.Collide))
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
                var hitObj = hit.collider.gameObject;
                if (hitObj)
                {
                    hitObj.SetActive(false);
                    lastHit = hitObj;
                }

            }

        }

    }
    public void MoveCamera(Vector3 inputVector)
    {
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
        }
        else
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel"); 
           
            if (Mathf.Abs(scrollInput) > 0.01f) 
            {
                float adjustedScrollInput = scrollInput * 100f;
                FOV -= adjustedScrollInput * 10f * Time.unscaledDeltaTime;
                FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
                gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
            }
        }

    }

    public void MoveToHouse(Structure targetHouse)
    {

        //Debug.Log($"Move to house {targetHouse.transform.position}");
        GameManager.Instance.canControlCam = false;
        camStartFOV = gameCamera.fieldOfView;
        targetFOV = 5f;
        Vector3 roadToHouse = (targetHouse.transform.position - targetHouse.roadPosition).normalized;
        targetPosition = targetHouse.roadPosition - (roadToHouse* focusDistance);
        targetPosition.y += 5f;

        target = targetHouse.transform;
        

        isFocusing = true;
    }

    public void ResetCam()
    {
        isFocusing = false;
        if (lastHit)
        {
            lastHit.SetActive(true);
            lastHit = null;
        }
        gameCamera.fieldOfView = camStartFOV;
        targetPosition = camStartPos;
        transform.rotation = camStartRotation;

        transform.SetPositionAndRotation(camStartPos, camStartRotation);
        GameManager.Instance.canControlCam = true;
    }
}
