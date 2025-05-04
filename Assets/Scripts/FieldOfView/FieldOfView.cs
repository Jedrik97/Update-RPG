using UnityEngine;
using System;
using System.Collections;

public class FieldOfView : MonoBehaviour
{
    private Transform player = null;
    private Coroutine fovCheckCoroutine = null;

    [Header("Field of View")]
    [SerializeField] private float viewRadius = 15f;
    [SerializeField, Range(0, 360)] private float viewAngle = 180f;

    [Header("Layers")]
    [SerializeField] private LayerMask obstacleMask;

    public event Action<bool> OnPlayerVisibilityChanged;
    public Transform Player => player;

    private bool playerVisible = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;

            if (fovCheckCoroutine == null)
                fovCheckCoroutine = StartCoroutine(CheckFOVRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fovCheckCoroutine != null)
            {
                StopCoroutine(fovCheckCoroutine);
                fovCheckCoroutine = null;
            }

            player = null;
            playerVisible = false;
            OnPlayerVisibilityChanged?.Invoke(false);
        }
    }

    private IEnumerator CheckFOVRoutine()
    {
        while (player)
        {
            if (CheckPlayerInFOV())
            {
                playerVisible = true;
                OnPlayerVisibilityChanged?.Invoke(true);
                yield break; 
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private bool CheckPlayerInFOV()
    {
        if (!player)
            return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < viewRadius)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayer < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f, false);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        Gizmos.color = Color.cyan;
        int segments = 50;
        float angleStep = viewAngle / segments;
        Vector3 previousPoint = transform.position + DirFromAngle(-viewAngle / 2f, false) * viewRadius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle / 2f + angleStep * i;
            Vector3 nextPoint = transform.position + DirFromAngle(angle, false) * viewRadius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal) angleInDegrees += transform.eulerAngles.y;
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }
#endif
}
