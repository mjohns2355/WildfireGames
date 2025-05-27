using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;
    [SerializeField] private BoxCollider movementBounds;
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
    [SerializeField]private Vector3 camStartPos;
    private Quaternion camStartRotation;
    private float camStartFOV,targetFOV;
    [SerializeField] private List<GameObject> hitObjects = new ();
    float touchDist = 0;
    float lastDist = 0;
    public LayerMask ignoreLayerMask;
    private Tween fovTween;
    Bounds bounds;
    private void Start()
    {
        camStartPos = transform.position;
        camStartRotation = transform.rotation;
        gameCamera = GetComponent<Camera>();
        gameCamera.fieldOfView = defaultFOV;
        FOV = gameCamera.fieldOfView;
        camPos = gameCamera.transform.position;
        bounds = movementBounds.bounds;
        camStartFOV = gameCamera.fieldOfView;
        //GameManager.Instance.inputManager.OnMouseHold += DragToMoveCamera;
        //GameManager.Instance.inputManager.OnMouseUp += ResetMousePosition;
    }

    private void LateUpdate()
    {
        if(isFocusing)
        {
            
            //gameCamera.fieldOfView = 5;
            //transform.position = targetPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraZoomToHouseSpeed * Time.deltaTime);
            gameCamera.transform.LookAt(target.position);

            gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, targetFOV, cameraZoomToHouseSpeed * Time.deltaTime);


            
            for (int i = 0; i < hitObjects.Count; i++)
                hitObjects[i].SetActive(true);
            hitObjects.Clear();

            Vector3 origin = transform.position;
            Vector3 dir = (target.position - origin).normalized;
            float distance = Vector3.Distance(origin, target.position);

            int layerMask = ignoreLayerMask;

            RaycastHit[] hits = Physics.RaycastAll(origin, dir, distance, layerMask,
                                                   QueryTriggerInteraction.Collide);

            
            foreach (var hit in hits)
            {
                var go = hit.collider.gameObject;
                if (go.activeSelf)
                {
                    go.SetActive(false);
                    hitObjects.Add(go);
                }
            }

        }

    }
    public void MoveCamera(Vector3 inputVector)
    {
        camPos += inputVector * Time.deltaTime * cameraMovementSpeed;

        if (movementBounds != null)
        {

            //Debug.Log($"Bounds: x({bounds.min.x:F1} → {bounds.max.x:F1}), z({bounds.min.z:F1} → {bounds.max.z:F1})");
            camPos.x = Mathf.Clamp(camPos.x, bounds.min.x, bounds.max.x);
            camPos.z = Mathf.Clamp(camPos.z, bounds.min.z, bounds.max.z);
        }

        transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime * lerpSpeed);
    }

    public void ZoomCamera()
    {    // Check for desktop input
        if (!GameManager.Instance.canControlCam) return;
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
                    if (fovTween != null && fovTween.IsActive()) fovTween.Kill();
                    fovTween = gameCamera.DOFieldOfView(FOV, 0.3f).SetEase(Ease.OutQuad);
                    // gameCamera.fieldOfView = FOV;
                    //gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
                }
            }
        }
        else
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel"); 
           
            if (Mathf.Abs(scrollInput) > 0.01f) 
            {
                float adjustedScrollInput = scrollInput * 100f;
                FOV -= adjustedScrollInput * 10f * Time.deltaTime;
                FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
                //gameCamera.fieldOfView = Mathf.SmoothDamp(gameCamera.fieldOfView, FOV, ref velocity, smoothTime);
                //gameCamera.fieldOfView = FOV;
                if (fovTween != null && fovTween.IsActive()) fovTween.Kill();
                fovTween = gameCamera.DOFieldOfView(FOV, 0.3f).SetEase(Ease.OutQuad);
            }
        }

    }

    public void MoveToHouse(Structure targetHouse, bool shouldLerp = true)
    {
        //Debug.Log($"Move to house {targetHouse.transform.position}");

        GameManager.Instance.canControlCam = false;
       
        targetFOV = 5f;
        Vector3 roadToHouse = (targetHouse.transform.position - targetHouse.roadPosition).normalized;
        targetPosition = targetHouse.roadPosition - (roadToHouse* focusDistance);
        targetPosition.y += 5f;

        target = targetHouse.transform;
        if (!shouldLerp)
        {
            transform.position = targetPosition;
            gameCamera.transform.LookAt(target.position);

            gameCamera.fieldOfView = targetFOV;
            return;
        }

        isFocusing = true;
    }

    public void ResetCam()
    {
        
        isFocusing = false;
        for (int i = 0; i < hitObjects.Count; i++)
            hitObjects[i].SetActive(true);
        hitObjects.Clear();

        gameCamera.fieldOfView = camStartFOV;
        FOV = camStartFOV;

        camPos = camStartPos;
        transform.SetPositionAndRotation(camStartPos, camStartRotation);

        if (fovTween != null && fovTween.IsActive())
        {
            fovTween.Kill();
            fovTween = null;
        }

        GameManager.Instance.canControlCam = true;
    }
}
