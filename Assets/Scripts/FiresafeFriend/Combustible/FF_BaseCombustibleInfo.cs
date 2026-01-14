using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "New Combustible Info", menuName = "Combustible Info")]
    public class FF_BaseCombustibleInfo : ScriptableObject
    {
        //partID will now be associated with the key
        public string partID;
        public float durability;
        public float flammability;
        public Sprite icon;
        public MaterialClass materialClass;
        [TextArea]
        public string description;

        public string GetLocalizedName()
        {
            if (StringManager.Instance == null) return partID;
            return StringManager.Instance.GetText(partID);
        }

        public string GetLocalizedDescription()
        {
            if (StringManager.Instance == null) return description;
            return StringManager.Instance.GetText(description);
        }
    }


