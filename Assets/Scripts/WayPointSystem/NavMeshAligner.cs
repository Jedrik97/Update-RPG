/*using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.SceneManagement;

public static class NavMeshAligner
{
    private const float kDefaultMaxDistance = 5f;

    [MenuItem("Tools/NavMesh/Align Selected To NavMesh %#a")]
    private static void AlignSelected()
    {
        var objs = Selection.transforms;
        if (objs.Length == 0)
        {
            Debug.LogWarning("NavMeshAligner: nothing selected");
            return;
        }

        bool anyMoved = false;
        foreach (var t in objs)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(t.position, out hit, kDefaultMaxDistance, NavMesh.AllAreas))
            {
                Undo.RecordObject(t, "Align To NavMesh");
                t.position = hit.position;
                anyMoved = true;
            }
            else
            {
                Debug.LogWarning($"'{t.name}': no NavMesh within {kDefaultMaxDistance} units of {t.position}");
            }
        }

        if (anyMoved)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("NavMeshAligner: selected objects aligned.");
        }
    }

    [MenuItem("Tools/NavMesh/Align Selected To NavMesh %#a", validate = true)]
    private static bool AlignSelected_Validate()
    {
        return Selection.transforms.Length > 0;
    }
}*/