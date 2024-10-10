using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New House Blueprint", menuName = "House System/House Blueprint")]
public class HouseBlueprint : ScriptableObject
{
    [System.Serializable]
    public class HousePartConnection
    {
        public string partID;
        public GameObject partPrefab;
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale;
        public List<string> connectedPartsId;
    }

    public List<HousePartConnection> partConnections;
}
