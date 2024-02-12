using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class io_brakes : MonoBehaviour
{
    public bool isBraking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartBrake()
    {
        isBraking = true;
    }
    public void EndBrake()
    {
        isBraking = false;
    }
}
