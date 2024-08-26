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

    float FOV;
    Vector3 camPos;
    private void Start()
    {
        gameCamera = GetComponent<Camera>();
        gameCamera.fieldOfView = defaultFOV;
        FOV = gameCamera.fieldOfView;
        camPos = gameCamera.transform.position;
    }
    public void MoveCamera(Vector3 inputVector)
    {

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
}
