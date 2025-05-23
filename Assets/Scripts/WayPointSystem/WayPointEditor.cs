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
        WayPoint selectedWayPoint = Selection.activeGameObject.GetComponent<WayPoint>(); // Получаем выбранный WayPoint.
        wayPointObj.transform.position = selectedWayPoint.transform.position; // Копируем позицию выбранного WayPoint.
        wayPointObj.transform.forward = selectedWayPoint.transform.forward; // Копируем направление выбранного WayPoint.
        wayPoint.PreviousWayPoint = selectedWayPoint; // Присваиваем текущий WayPoint как предыдущий.
        if (selectedWayPoint.NextWayPoint) // Если есть следующий WayPoint.
        {
            selectedWayPoint.NextWayPoint.PreviousWayPoint = wayPoint; // Связываем следующий WayPoint с новым.
            wayPoint.NextWayPoint = wayPoint; // Связываем новый WayPoint с текущим как следующий.
        }

        selectedWayPoint.NextWayPoint = wayPoint; // Обновляем связь текущего WayPoint.
        wayPoint.transform.SetSiblingIndex(selectedWayPoint.transform
            .GetSiblingIndex()); // Устанавливаем порядок в иерархии.
        Selection.activeGameObject = wayPoint.gameObject; // Делаем новый WayPoint активным.
    }

    private void CreateWayPoint() // Метод для создания нового WayPoint.
    {
        GameObject wayPointObj =
            new GameObject("WayPoint" + WayPointsRoot.childCount,
                typeof(WayPoint)); // Создаём новый объект с компонентом WayPoint.
        wayPointObj.transform.SetParent(WayPointsRoot, false); // Назначаем родителя для нового объекта.
        WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>(); // Получаем компонент WayPoint у нового объекта.
        if (WayPointsRoot.childCount > 1) // Если в корне есть другие WayPoint.
        {
            wayPoint.PreviousWayPoint =
                WayPointsRoot.GetChild(WayPointsRoot.childCount - 2)
                    .GetComponent<WayPoint>(); // Присваиваем предыдущий WayPoint.
            wayPoint.PreviousWayPoint.NextWayPoint = wayPoint; // Связываем предыдущий WayPoint с новым.
            wayPoint.transform.position = wayPoint.PreviousWayPoint.transform.position; // Копируем позицию предыдущего.
            wayPoint.transform.forward =
                wayPoint.PreviousWayPoint.transform.forward; // Копируем направление предыдущего.
        }

        Selection.activeObject = wayPoint.gameObject; // Делаем новый WayPoint активным.
    }
}