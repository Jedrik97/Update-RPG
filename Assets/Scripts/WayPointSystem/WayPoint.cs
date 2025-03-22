using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public WayPoint PreviousWayPoint;
    public WayPoint NextWayPoint;
    [UnityEngine.Range(0f, 5f)] public float Width = 3f;
    public List<WayPoint> WayPoints = new List<WayPoint>();

    public Vector3 GetPositionWayPoint()
    {
        Vector3 minBounds = transform.position + transform.right * Width / 2f;
        Vector3 maxBounds = transform.position - transform.right * Width / 2f;
        return Vector3.Lerp(minBounds, maxBounds, Random.Range(0f, 1f));
    }
}