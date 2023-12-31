using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_flowerbed : MonoBehaviour
{
    private void OnMouseDown()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
