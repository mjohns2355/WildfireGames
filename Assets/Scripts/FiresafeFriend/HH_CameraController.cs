using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HH_CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float moveSpeed = 5f; 
    public float zoomDistance = 5f;
    public float maxFOV = 70;
    public Vector3 camPosOffset = Vector3.zero;
    private Vector3 targetPosition;
    private bool isZooming,shouldLerp = false;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private float defaultFOV;
    private void Awake()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
        defaultFOV = Camera.main.fieldOfView;   
    }
    private void Start()
    {
        
        HH_GameManager.Instance.inputManager.OnHouseSelected += MoveToHouse;
    }
    // Update is called once per frame
    void Update()
    {
        if (isZooming)
        {
            if (shouldLerp)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, maxFOV, zoomSpeed * Time.deltaTime);

            
            if (shouldLerp && Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isZooming = false;
                //HH_GameManager.Instance.inputManager.OnHouseSelected -= MoveToHouse;
            }
        }
    }


    public void MoveToHouse(HouseManager targetHouse)
    {
        Debug.Log($"Move to house {targetHouse.playerTag}");
        //targetPosition = targetHouse.transform.position + camPosOffset - targetHouse.transform.forward * zoomDistance;
        var targetCamTransform = targetHouse.playerTag == "P1" ? HH_GameManager.Instance.h1CamPos : HH_GameManager.Instance.h2CamPos;
        Zoomcamera(targetCamTransform);
    }

    public void Zoomcamera(Transform targetTransform,bool shouldLerp = true, float maxFov = 70)
    {
        maxFOV = maxFov;
        this.shouldLerp = shouldLerp;
        targetPosition = targetTransform.position;
        Camera.main.transform.rotation = targetTransform.rotation;
        if(!shouldLerp)
        {
            transform.position = targetPosition;
        }
        isZooming = true;
    }

    public void ResetCamera()
    {
        Debug.Log("Reset Camera");
        
        transform.SetPositionAndRotation(defaultPosition, defaultRotation);
        Camera.main.fieldOfView = defaultFOV;
    }
}
