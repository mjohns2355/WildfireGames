using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    public HouseBlueprint houseBlueprint;
    private void Start()
    {
        InitializeDefaultHouseLayout();
    }

    void InitializeDefaultHouseLayout()
    {
        foreach (var part in houseBlueprint.partConnections)
        {
            var obj = Instantiate(part.partPrefab, transform);
            obj.transform.localPosition = part.localPosition;
            //Debug.Log(part.localRotation);
            obj.transform.rotation = new Quaternion(part.localRotation.x, part.localRotation.y, part.localRotation.z, Quaternion.identity.w);
            //obj.transform.localRotation = new Quaternion(part.localRotation.x, part.localRotation.y, part.localRotation.z, Quaternion.identity.w);
            obj.transform.localScale = part.localScale;
        }
    }
}
