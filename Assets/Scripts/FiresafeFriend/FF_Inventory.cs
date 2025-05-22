using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HappyHouse.HouseSystem
{
    public class FF_Inventory : MonoBehaviour
    {
        public Dictionary<HousePartType, List<HousePartInfo>> ownedParts = new Dictionary<HousePartType, List<HousePartInfo>>();
        public Dictionary<HousePartType, List<HousePartInfo>> ownedPublicParts = new Dictionary<HousePartType, List<HousePartInfo>>();

        public bool AddNewPartToInventory(HousePartInfo newPartInfo)
        {
            if(newPartInfo == null) return false;
            var targetDict = newPartInfo.isPublic ? ownedPublicParts : ownedParts;
            if (targetDict.ContainsKey(newPartInfo.housePartType))
            {
                var value = targetDict[newPartInfo.housePartType];

                if (value.Exists(part => part.partID == newPartInfo.partID)) return false;
                value.Add(Instantiate(newPartInfo));
            }
            else
            {
                targetDict.Add(newPartInfo.housePartType, new List<HousePartInfo> { newPartInfo });
            }
            if(newPartInfo.isPublic)
            {
                Debug.Log($"Added {newPartInfo.name} into inventory");
            }

            return true;
        }

        public bool RemovePartFromInventory(HousePartType partToRemove,string partID, bool isPublic = false)
        {
            //if (partToRemove == null) return false;
            var targetDict = isPublic ? ownedPublicParts : ownedParts;
            if (targetDict.ContainsKey(partToRemove))
            {
                var value = targetDict[partToRemove];

                // Find the part with the matching ID and remove it
                var part = value.FirstOrDefault(p => p.partID == partID);
                if (part != null)
                {
                    value.Remove(part);

                    if (value.Count == 0)
                    {
                        targetDict.Remove(partToRemove);
                    }
                    //Debug.Log($"Removed {partID} from inventory");
                    return true;
                }
            }

            return false;
        }
        public bool PlayerOwnsPart(HousePartInfo part)
        {
            var targetDict = part.isPublic ? ownedPublicParts : ownedParts;
            //Debug.Log($"Check {part.partID}");
            if (targetDict.ContainsKey(part.housePartType))
            {
                var value = targetDict[part.housePartType];
                return targetDict[part.housePartType].Exists(p => p.partID == part.partID);
            }
            return false;
        }


    }
}

