using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : UnitySingleton<ResourceManager>
{
    public Dictionary<HousePartType, List<HousePart>> allAvailableParts;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        InitPartsDictionary();
    }

    private void InitPartsDictionary()
    {
        allAvailableParts = new Dictionary<HousePartType, List<HousePart>> ();
        var allParts = Resources.LoadAll<HousePart>("ResillientResident/HousePartsSO");
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
                allAvailableParts.Add(part.housePartType, new List<HousePart> { part }); // Add new key-value pair
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
