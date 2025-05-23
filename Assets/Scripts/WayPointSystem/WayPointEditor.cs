using UnityEditor;
using UnityEngine;

public class WayPointEditor : EditorWindow
{
    [MenuItem("Waypoint Tools/WayPoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointEditor>();
    }

    public Transform WayPointsRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("WayPointsRoot"));
        if (!WayPointsRoot)
        {
            EditorGUILayout.HelpBox("No WayPoints Root is assigned.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButton();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButton()
    {
        if (GUILayout.Button("Add WayPoint"))
        {
            CreateWayPoint();
        }

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<WayPoint>())
        {
            if (GUILayout.Button("Add NextWayPoint"))
            {
                CreateWayPointAfter();
                if (GUILayout.Button("Previous WayPoint"))
                {
                    CreateWayPointBefore();
                }
            }
        }
    }

    private void CreateWayPointBefore()
    {
        GameObject wayPointObj = new GameObject("WayPoint" + WayPointsRoot.childCount, typeof(WayPoint));
        wayPointObj.transform.SetParent(WayPointsRoot, false);
        WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>();
        WayPoint selectedWayPoint = Selection.activeGameObject.GetComponent<WayPoint>();
        wayPointObj.transform.position = selectedWayPoint.transform.position;
        wayPointObj.transform.forward = selectedWayPoint.transform.forward;
        if (selectedWayPoint.PreviousWayPoint)
        {
            wayPoint.PreviousWayPoint = selectedWayPoint.PreviousWayPoint;
            selectedWayPoint.PreviousWayPoint.NextWayPoint = wayPoint;
        }

        wayPoint.NextWayPoint = selectedWayPoint;
        selectedWayPoint.PreviousWayPoint = wayPoint;
        wayPoint.transform.SetSiblingIndex(selectedWayPoint.transform.GetSiblingIndex());
        Selection.activeGameObject = wayPoint.gameObject;
    }

    private void CreateWayPointAfter()
    {
        GameObject wayPointObj = new GameObject("WayPoint" + WayPointsRoot.childCount, typeof(WayPoint));
        wayPointObj.transform.SetParent(WayPointsRoot, false);
        WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>();
        WayPoint selectedWayPoint = Selection.activeGameObject.GetComponent<WayPoint>(); 
        wayPointObj.transform.position = selectedWayPoint.transform.position; 
        wayPointObj.transform.forward = selectedWayPoint.transform.forward; 
        wayPoint.PreviousWayPoint = selectedWayPoint; 
        if (selectedWayPoint.NextWayPoint) 
        {
            selectedWayPoint.NextWayPoint.PreviousWayPoint = wayPoint; 
            wayPoint.NextWayPoint = wayPoint; 
        }

        selectedWayPoint.NextWayPoint = wayPoint; 
        wayPoint.transform.SetSiblingIndex(selectedWayPoint.transform
            .GetSiblingIndex()); 
        Selection.activeGameObject = wayPoint.gameObject; 
    }

    private void CreateWayPoint() 
    {
        GameObject wayPointObj =
            new GameObject("WayPoint" + WayPointsRoot.childCount,
                typeof(WayPoint)); 
        wayPointObj.transform.SetParent(WayPointsRoot, false); 
        WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>(); 
        if (WayPointsRoot.childCount > 1) 
        {
            wayPoint.PreviousWayPoint =
                WayPointsRoot.GetChild(WayPointsRoot.childCount - 2)
                    .GetComponent<WayPoint>(); 
            wayPoint.PreviousWayPoint.NextWayPoint = wayPoint; 
            wayPoint.transform.position = wayPoint.PreviousWayPoint.transform.position; 
            wayPoint.transform.forward =
                wayPoint.PreviousWayPoint.transform.forward; 
        }

        Selection.activeObject = wayPoint.gameObject; 
    }
}