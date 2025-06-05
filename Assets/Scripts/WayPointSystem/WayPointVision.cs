/*using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WayPointVision
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawGizmo(WayPoint wayPoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.green * 0.5f;
        }

        Gizmos.DrawSphere(wayPoint.transform.position, 0.2f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(wayPoint.transform.position + (wayPoint.transform.right * wayPoint.Width / 2),
            wayPoint.transform.position - (wayPoint.transform.right * wayPoint.Width / 2));
        if (wayPoint.PreviousWayPoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 offset = wayPoint.transform.right * wayPoint.Width / 2;
            Vector3 offsetTo = wayPoint.PreviousWayPoint.transform.right * wayPoint.PreviousWayPoint.Width / 2;
            Gizmos.DrawLine(wayPoint.transform.position + offset,
                wayPoint.PreviousWayPoint.transform.position + offsetTo);
        }

        if (wayPoint.NextWayPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector3 offset = wayPoint.transform.right * -wayPoint.Width / 2;
            Vector3 offsetTo = wayPoint.NextWayPoint.transform.right * -wayPoint.NextWayPoint.Width / 2;
            Gizmos.DrawLine(wayPoint.transform.position + offset, wayPoint.NextWayPoint.transform.position + offsetTo);
        }
    }
}*/