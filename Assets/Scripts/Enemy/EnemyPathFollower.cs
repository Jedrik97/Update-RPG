using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 1f;

    public delegate void PatrolCompleted();
    public event PatrolCompleted OnPatrolCompleted;

    private float waitTime = 2f;
    private float waitTimer = 0f;
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isChasing = false;
    private bool isWaiting = false;

    private NavMeshAgent agent;
    private EnemyBase enemy;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyBase>();
        agent.isStopped = false;
        isChasing = false;
        isWaiting = false;
        currentWaypointIndex = 0;
        movingForward = true;

        enemy.OnDeath += HandleDeath;

        // Начинаем патрулирование сразу, если заданы точки
        if (waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void OnDisable()
    {
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
        agent.isStopped = true;
    }

    public void ResumePatrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        isChasing = false;
        isWaiting = false;
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

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
        if (movingForward)
        {
            if (currentWaypointIndex < waypoints.Length - 1)
                currentWaypointIndex++;
            else
            {
                movingForward = false;
                OnPatrolCompleted?.Invoke();
            }
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

    public void SetWaypoints(WayPoint[] newWaypoints)
    {
        waypoints = new Transform[newWaypoints.Length];
        for (int i = 0; i < newWaypoints.Length; i++)
        {
            waypoints[i] = newWaypoints[i].transform;
        }
    }

    public void ResetWaypoints()
    {
        waypoints = new Transform[0];
    }

    private void HandleDeath(GameObject enemyObj)
    {
        ResetWaypoints();
    }
}
