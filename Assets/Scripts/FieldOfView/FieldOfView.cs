using UnityEngine;
using System;

public class FieldOfView : MonoBehaviour
{
    private Transform player;

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
            CheckPlayerInFOV();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            OnPlayerVisibilityChanged?.Invoke(false);
        }
    }

    private void FixedUpdate()
    {
        if (player)
        {
            bool isVisible = CheckPlayerInFOV();
            if (isVisible != playerVisible)
            {
                playerVisible = isVisible;
                OnPlayerVisibilityChanged?.Invoke(playerVisible);
            }
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
}
