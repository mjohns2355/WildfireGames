using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HappyHouse.HouseSystem
{
    [CreateAssetMenu(fileName = "New House Part", menuName = "House System/House Part")]
    public class HousePart : ScriptableObject
    {
        public HousePartType housePartType;
        public float durability;
        public float flammability;
        public MaterialType materialType;
        public Material material;
        public GameObject mesh;
        public float price;
    }
}


