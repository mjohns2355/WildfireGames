using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;
    public float cameraZoomSpeed = 5;
    [SerializeField] private float maxFOV;
    [SerializeField] private float minFOV;
    [SerializeField] private float defaultFOV;
    private void Start()
    {
        gameCamera = GetComponent<Camera>();
        gameCamera.fieldOfView = defaultFOV;
    }
    public void MoveCamera(Vector3 inputVector)
    {
        //var movementVector = Quaternion.Euler(0, 30, 0) * inputVector;
        //gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        gameCamera.transform.position += inputVector * Time.deltaTime * cameraMovementSpeed;
    }

    public void ZoomCamera(float mouseAxis)
    {
        
        float FOV = gameCamera.fieldOfView;
        //FOV += mouseAxis * -1 * cameraMovementSpeed;
        FOV += mouseAxis * -1 * 0.5f;
        FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
        gameCamera.fieldOfView =FOV;
    }
}
