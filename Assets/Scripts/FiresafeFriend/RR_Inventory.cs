using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HappyHouse.HouseSystem
{
    public class RR_Inventory : MonoBehaviour
    {
        public Dictionary<HousePartType, List<HousePartInfo>> ownedParts = new Dictionary<HousePartType, List<HousePartInfo>>();

        public bool AddNewPartToInventory(HousePartInfo newPartInfo)
        {
            if(newPartInfo == null) return false;
            if (ownedParts.ContainsKey(newPartInfo.housePartType))
            {
                var value = ownedParts[newPartInfo.housePartType];

                if (value.Exists(part => part.partID == newPartInfo.partID)) return false;
                value.Add(Instantiate(newPartInfo));
            }
            else
            {
                ownedParts.Add(newPartInfo.housePartType, new List<HousePartInfo> { newPartInfo });
            }
            //Debug.Log($"Added {newPartInfo.partID} to inventory");
            return true;
        }

        public bool RemovePartFromInventory(HousePartInfo partToRemove)
        {
            if (partToRemove == null) return false;

            if (ownedParts.ContainsKey(partToRemove.housePartType))
            {
                var value = ownedParts[partToRemove.housePartType];

                // Find the part with the matching ID and remove it
                var part = value.FirstOrDefault(p => p.partID == partToRemove.partID);
                if (part != null)
                {
                    value.Remove(part);

                    if (value.Count == 0)
                    {
                        ownedParts.Remove(partToRemove.housePartType);
                    }
                    Debug.Log($"Removed {partToRemove.partID} from inventory");
                    return true;
                }
            }

            return false;
        }
        public bool PlayerOwnsPart(HousePartInfo part)
        {
            //Debug.Log($"Check {part.partID}");
            if (ownedParts.ContainsKey(part.housePartType))
            {
                var value = ownedParts[part.housePartType];
                return ownedParts[part.housePartType].Exists(p => p.partID == part.partID);
            }
            return false;
        }


    }
}

