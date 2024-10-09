using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New House Blueprint", menuName = "House System/House Blueprint")]
public class HouseBlueprint : ScriptableObject
{
    [System.Serializable]
    public class HousePartConnection
    {
        public HousePart part;
        public List<HousePart> connectedParts;
    }

    public List<HousePartConnection> partConnections;
}
