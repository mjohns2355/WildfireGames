using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : UnitySingleton<ResourceManager>
{
    public Dictionary<HousePartType, List<HousePartInfo>> allAvailableParts;
    public Dictionary<HousePartType, GameObject> VFXs;
    public GameObject[] plants;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        InitPartsDictionary();
        LoadVFXs();
        LoadPlants();
    }

    private void InitPartsDictionary()
    {
        allAvailableParts = new Dictionary<HousePartType, List<HousePartInfo>> ();
        var allParts = Resources.LoadAll<HousePartInfo>("FiresafeFriend/HousePartsSO");
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

    private void LoadVFXs()
    {
        VFXs = new Dictionary<HousePartType, GameObject> ();

        var allVFXs = Resources.LoadAll<GameObject>("FiresafeFriend/DestroyEffects");
        foreach(var vfx in allVFXs)
        {
            HousePartType type;
            if(Enum.TryParse(vfx.name, out type))
            {
                Debug.Log(type.ToString());
                if (VFXs.ContainsKey(type))
                {
                    VFXs[type] = vfx;
                }
                else
                {
                    VFXs.Add(type, vfx);
                }
            }
            else
            {
                Debug.Log("Parse Enum Failed");
            }
        }

    }

    private void LoadPlants() 
    { 
        plants = Resources.LoadAll<GameObject>("FiresafeFriend/PlantsPrefab");
    }

}
