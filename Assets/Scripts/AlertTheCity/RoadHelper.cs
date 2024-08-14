using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHelper : MonoBehaviour
{
    [SerializeField]
    protected List<Marker> InnerCarMarkers;
    [SerializeField]
    protected List<Marker> OuterCarMarkers;
    [SerializeField]
    protected bool isCorner;
    [SerializeField]
    protected bool hasCrosswalks;

    public bool useInner;
    
    float approximateThresholdCorner = 0.3f;

    [SerializeField]
    private Marker innerIncoming, innerOutgoing, outerIncoming, outerOutgoing;

    //public virtual Marker GetpositioForCarToSpwan(Vector3 nextPathPosition)
    //{
    //    return innerOutgoing;
    //}
    //public virtual Marker GetpositioForCarToEnd(Vector3 previousPathPosition)
    //{
    //    return innerIncoming;
    //}

    public virtual Marker GetPositioForCarToSpawn(Vector3 nextPathPosition)
    {
        return useInner ? innerOutgoing : outerOutgoing;
    }

    public virtual Marker GetPositioForCarToEnd(Vector3 previousPathPosition)
    {
        return useInner ? innerIncoming : outerIncoming;
    }

    protected Marker GetClosestMarkeTo(Vector3 structurePosition, List<Marker> pedestrianMarkers, bool isCorner = false)
    {
        if (isCorner)
        {
            foreach (var marker in pedestrianMarkers)
            {
                var direction = marker.Position - structurePosition;
                direction.Normalize();
                if (Mathf.Abs(direction.x) < approximateThresholdCorner || Mathf.Abs(direction.z) < approximateThresholdCorner)
                {
                    return marker;
                }
            }
            return null;
        }
        else
        {
            Marker closestMarker = null;
            float distance = float.MaxValue;
            foreach (var marker in pedestrianMarkers)
            {
                var markerDistance = Vector3.Distance(structurePosition, marker.Position);
                if (distance > markerDistance)
                {
                    distance = markerDistance;
                    closestMarker = marker;
                }
            }
            return closestMarker;
        }
    }


    public Vector3 GetClosestCarMarkerPosition(Vector3 currentPosition)
    {
        var carMarkers = useInner? InnerCarMarkers : OuterCarMarkers;
        return GetClosestMarkeTo(currentPosition, carMarkers, false).Position;
    }


    public List<Marker> GetAllCarMarkers()
    {
        return useInner ? InnerCarMarkers : OuterCarMarkers;
    }
}
