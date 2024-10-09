using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New House Part", menuName = "House System/House Part")]
public class HousePart:ScriptableObject
{
    public string partName;
    public float durability;
    public float flammability;
}
