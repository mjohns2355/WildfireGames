
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHelperMultipleCarMarkers : RoadHelper
{
    [SerializeField]
    protected List<Marker> innerIncommingMarkers, innerOutgoingMarkers, outerIncomingMarkers,  outerOutgoingMarkers;

    public override Marker GetPositioForCarToSpawn(Vector3 nextPathPosition)
    {
        return useInner ? GetClosestMarkeTo(nextPathPosition, innerOutgoingMarkers) : GetClosestMarkeTo(nextPathPosition, outerOutgoingMarkers);
        // GetClosestMarkeTo(nextPathPosition, outgoingMarkers);
    }

    public override Marker GetPositioForCarToEnd(Vector3 previousPathPosition)
    {
        return useInner ? GetClosestMarkeTo(previousPathPosition, innerIncommingMarkers) : GetClosestMarkeTo(previousPathPosition, outerIncomingMarkers);
    }
}
