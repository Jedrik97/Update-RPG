using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 1f;
    [SerializeField] private float patrolSpeed = 2f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isChasing = false;
    private bool isWaiting = false;
    private float waitTime = 2f;
    private float waitTimer = 0f;

    private NavMeshAgent agent;
    private EnemyBase enemy;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyBase>();
        isChasing = false;
        isWaiting = false;
        currentWaypointIndex = 0;
        movingForward = true;
        enemy.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if(agent != null)
            agent.isStopped = true;
        isChasing = false;
        enemy.OnDeath -= HandleDeath;
    }

    private void FixedUpdate()
    {
        if (!isChasing)
            Patrol();
    }

    public void StopPatrol()
    {
        isChasing = true;
        if(agent != null)
            agent.isStopped = true;
    }

    public void ResumePatrol()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;
        if (agent == null)
            return;

        agent.speed = patrolSpeed;
        if (!agent.isOnNavMesh)
        {
            StartCoroutine(WaitForNavMeshAndResume());
            return;
        }
        isChasing = false;
        isWaiting = false;
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private IEnumerator WaitForNavMeshAndResume()
    {
        yield return new WaitUntil(() => agent.isOnNavMesh);
        ResumePatrol();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;
        if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    MoveToNextPoint();
                }
            }
        }
    }

    private void MoveToNextPoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

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
        if(agent != null)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public void SetWaypoints(WayPoint[] newWaypoints)
    {
        waypoints = new Transform[newWaypoints.Length];
        for (int i = 0; i < newWaypoints.Length; i++)
            waypoints[i] = newWaypoints[i].transform;
    }

    private void HandleDeath(GameObject enemyObj)
    {
        waypoints = new Transform[0];
        isChasing = true;
        if(agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }
}
