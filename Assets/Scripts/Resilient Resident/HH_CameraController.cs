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
    public float maxFOV = 50;
    public Vector3 camPosOffset = Vector3.zero;
    private Vector3 targetPosition;
    private bool isZooming = false;
    void OnEnable()
    {
        HH_InputManager.Instance.OnHouseSelected += MoveToHouse;
    }



    void OnDisable()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (isZooming)
        {
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, maxFOV, zoomSpeed * Time.deltaTime);

            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isZooming = false;
                HH_InputManager.Instance.OnHouseSelected -= MoveToHouse;
            }
        }
    }


    private void MoveToHouse(HouseManager targetHouse)
    {
        targetPosition = targetHouse.transform.position + camPosOffset - targetHouse.transform.forward * zoomDistance;
        isZooming = true;
    }
}
