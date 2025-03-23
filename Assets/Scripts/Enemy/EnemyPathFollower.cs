using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachThreshold = 0.5f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private Vector3 lastPatrolPoint;
    private bool isChasing = false;
    
    private NavMeshAgent agent;
    private EnemyMeleeAI meleeAI;
    private EnemyBase enemy;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        meleeAI = GetComponent<EnemyMeleeAI>();
        enemy = GetComponent<EnemyBase>();

        if (waypoints.Length > 0)
        {
            lastPatrolPoint = waypoints[0].position;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (meleeAI)
        {
            meleeAI.OnStopPatrol += StopPatrol;
            meleeAI.OnReturnToPatrol += ReturnToPatrol;
        }
        
        enemy.OnHealthChanged += OnEnemyHealthChanged;
    }

    private void OnDestroy()
    {
        if (meleeAI)
        {
            meleeAI.OnStopPatrol -= StopPatrol;
            meleeAI.OnReturnToPatrol -= ReturnToPatrol;
        }
        
        enemy.OnHealthChanged -= OnEnemyHealthChanged;
    }

    private void Update()
    {
        if (!isChasing)
        {
            Patrol();
        }
    }

    private void StopPatrol()
    {
        isChasing = true;
        agent.isStopped = true;
    }

    private void ReturnToPatrol()
    {
        isChasing = false;
        agent.isStopped = false;
        agent.SetDestination(lastPatrolPoint);
        StartCoroutine(enemy.GradualHeal());
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

    private void OnEnemyHealthChanged(float currentHealth)
    {
        if (currentHealth == enemy.maxHealth)
        {
            // Если здоровье восстановлено, активируем движение
            if (!isChasing && agent.isStopped)
            {
                agent.isStopped = false;
                agent.SetDestination(waypoints[currentWaypointIndex].position);  // Возвращаем врага к текущей точке патруля
            }
        }
    }
}
