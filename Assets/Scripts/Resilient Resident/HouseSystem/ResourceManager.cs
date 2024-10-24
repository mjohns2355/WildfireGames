using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : UnitySingleton<ResourceManager>
{
    public Dictionary<HousePartType, List<HousePartInfo>> allAvailableParts;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        InitPartsDictionary();
    }

    private void InitPartsDictionary()
    {
        allAvailableParts = new Dictionary<HousePartType, List<HousePartInfo>> ();
        var allParts = Resources.LoadAll<HousePartInfo>("ResillientResident/HousePartsSO");
        Debug.Log(allParts.Length);
        foreach (var part in allParts)
        {
           
            if (allAvailableParts.ContainsKey(part.housePartType))
            {
                var value = allAvailableParts[part.housePartType];
                if (!value.Contains(part))
                {
                    value.Add(part);
                }
            }
            else
            {
                allAvailableParts.Add(part.housePartType, new List<HousePartInfo> { part }); // Add new key-value pair
            }
        }
    }


}
