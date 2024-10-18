using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
public class HH_InputManager : UnitySingleton<HH_InputManager>
{
    public Action OnHousePartSelected;
    public Action<HouseManager> OnHouseSelected;
    public bool canClickHouse = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();
    }

    void DetectInput()
    {
        // Handle mouse click for PC
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("House") && canClickHouse)
                {
                    var house = hit.collider.transform.GetComponent<HouseManager>();
                    OnHouseSelected?.Invoke(house); 
                }

                if(hit.collider.gameObject.layer == 10)
                {
                   
                    OnHousePartSelected?.Invoke();
                }
            }
        }

        // Handle touch input for mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Input.GetTouch(0).position;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("House"))
                {
                    var house = hit.collider.transform.GetComponent<HouseManager>();
                    OnHouseSelected?.Invoke(house);
                }
            }
        }
    }
}
