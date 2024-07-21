using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_collectable : MonoBehaviour
{
    private void OnMouseUp()
    {
        if(GameObject.FindGameObjectWithTag("BagPanel") == null)
            GameObject.FindGameObjectWithTag("PickupObject").GetComponent<FYTPickUp>().OpenPopup(gameObject);
      //  GameObject.FindGameObjectWithTag("Bag").GetComponent<FYT_Bag>().AddItem(gameObject.name);
      //  Destroy(gameObject);
    }


}
