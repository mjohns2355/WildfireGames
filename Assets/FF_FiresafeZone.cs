using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.FireSystem;
public class FF_FiresafeZone : MonoBehaviour
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
        if (other.gameObject.TryGetComponent<FF_FireController>(out var fire))
        {
            Debug.Log($"fire touches fire safe zone");
            fire.gameObject.SetActive(false);
        }
    }
}
