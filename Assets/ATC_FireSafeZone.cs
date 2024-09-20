using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_FireSafeZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var combustible = other.gameObject.GetComponent<Combustible>();
        var fire = other.gameObject.GetComponent<FireMovementController>();
        if (combustible != null)
        {
            //Debug.Log($"Fire Safe Zone {other.gameObject.name} is no longer combustible. ");
            combustible.enabled = false;
        }
        if(fire != null)
        {
            fire.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var fire = other.gameObject.GetComponent<FireMovementController>();
        if (fire != null)
        {
            fire.gameObject.SetActive(true);
        }
    }
}
