using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHouse.HouseSystem
{
    [CreateAssetMenu(fileName = "New Combustible Info", menuName = "Combustible Info")]
    public class BaseCombustibleInfo : ScriptableObject
    {
        public string partID;
        public float durability;
        public float flammability;
        public Sprite icon;
        [TextArea]
        public string description;
    }
}

