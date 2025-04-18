using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 1f;
    
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
        
        enemy.OnDeath += HandleDeath;
        
        Patrol();
    }

    private void OnDisable()
    {
        agent.isStopped = false;
        isChasing = false;
        enemy.OnDeath -= HandleDeath;
    }

    private void FixedUpdate()
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

    public void Patrol()
    {
        if (waypoints.Length == 0) return;

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
    private void HandleDeath(GameObject enemy)
    {
        ResetWaypoints();
    }
    
}
