using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HappyHouse.HouseSystem
{
    [CreateAssetMenu(fileName = "New House Part Info", menuName = "House System/House Part Info")]
    public class HousePartInfo : FF_BaseCombustibleInfo
    {
        //public string partID;
        public HousePartType housePartType;
        //public Sprite icon;
        //public float durability;
        //public float flammability;
        //public MaterialType materialType;
        public Material material;
        //public GameObject mesh;
        public float price;
        //public MaterialClass partClass;
        public bool isPublic;
        //[TextArea]
        //public string description;
    }
}


