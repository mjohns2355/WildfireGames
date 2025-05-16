using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "New Combustible Info", menuName = "Combustible Info")]
    public class FF_BaseCombustibleInfo : ScriptableObject
    {
        public string partID;
        public float durability;
        public float flammability;
        public Sprite icon;
        public MaterialClass materialClass;
        [TextArea]
        public string description;
    }


