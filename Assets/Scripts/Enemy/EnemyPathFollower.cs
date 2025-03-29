using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 1f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isChasing = false;

    private NavMeshAgent agent;
    private EnemyBase enemy;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyBase>();
        Patrol();
    }

    private void OnDisable()
    {
        agent.isStopped = false;
        isChasing = false;
    }

    private void Update()
    {
        if (!isChasing)
        {
            Patrol();
        }
        else
        {
            StopPatrol();
        }
    }

    public void StopPatrol()
    {
        isChasing = true;
    }

    public void ReturnToPatrol()
    {
        isChasing = false;  
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        
    }

    public void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            if (movingForward)
            {
                if (currentWaypointIndex < waypoints.Length - 1)
                    currentWaypointIndex++;
                else
                    movingForward = false;  
            }
            else
            {
                if (currentWaypointIndex > 0)
                    currentWaypointIndex--;
                else
                    movingForward = true;
            }

            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
