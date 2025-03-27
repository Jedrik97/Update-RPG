using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 0.5f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isChasing = false;

    private NavMeshAgent agent;
    private EnemyBase enemy;

    private void OnEnable()
    {
        OneSecond();
        isChasing = false;
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyBase>();
    }

    private void OnDisable()
    {
        isChasing = false;
    }

    private void Update()
    {
        if (!isChasing)
        {
            Patrol();
        }
    }

    public void StopPatrol()
    {
        isChasing = true;
        agent.isStopped = true;
    }

    public void ReturnToPatrol()
    {
        isChasing = false;  
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        if (enemy)
        {
            enemy.GradualHeal();
        }
    }

    private void Patrol()
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

    private IEnumerator OneSecond()
    {
        yield return new WaitForSeconds(1f);

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
