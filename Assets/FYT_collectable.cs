using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_collectable : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().AddItem(gameObject.name);
        Destroy(gameObject);
    }
}
