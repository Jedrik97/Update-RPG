using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMeleeAI : EnemyBase
{
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private EnemyPathFollower pathFollower;

    private NavMeshAgent agent;
    private Transform player;

    private bool hasSeenPlayer = false;
    private Vector3 chaseStartPoint;

    private float timeSinceLastAttack = 0f;
    private bool isReturning = false;

    public float attackRange = 2f;
    public float chaseSpeed = 3.5f;
    public float attackDelay = 1f;
    public float attackCooldown = 2f;
    public float maxChaseDistance = 15f; 

    private enum EnemyState { Idle, Patrolling, Chasing, Attacking, Returning }
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();

        if (fieldOfView != null)
        {
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
        }
    }

    private void OnDisable()
    {
        if (fieldOfView != null)
        {
            fieldOfView.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;
        }
    }

    private void HandlePlayerVisibilityChanged(bool isVisible)
    {
        if (isVisible)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                chaseStartPoint = transform.position;
            }
            player = fieldOfView.Player;
            
            if (player == null)
            {
                Debug.LogWarning("Player not found!");
                return;
            }

            StopPatrolling();
            currentState = EnemyState.Chasing;
        }
        else
        {
            hasSeenPlayer = false;
            currentState = EnemyState.Returning;
            StartCoroutine(ReturnToPatrol());
        }
    }

    private void StopPatrolling()
    {
        if (pathFollower != null) pathFollower.StopPatrol();
        agent.isStopped = false;
    }

    private void StartPatrolling()
    {
        if (pathFollower != null) pathFollower.Patrol();
        agent.isStopped = false;
    }

    private IEnumerator ReturnToPatrol()
    {
        agent.SetDestination(chaseStartPoint);
        while (Vector3.Distance(transform.position, chaseStartPoint) > 1f)
        {
            yield return null;
        }
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        currentState = EnemyState.Patrolling;
        StartPatrolling();
    }

    private void FixedUpdate()
    {
        timeSinceLastAttack += Time.fixedDeltaTime;

        if (player == null)
        {
            player = fieldOfView?.Player;
            if (player == null) return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (distanceToPlayer <= maxChaseDistance && hasSeenPlayer)
                {
                    currentState = EnemyState.Chasing;
                    StopPatrolling();
                }
                else if (pathFollower != null)
                {
                    StartPatrolling();
                }
                break;

            case EnemyState.Chasing:
                if (distanceFromStart > maxChaseDistance)
                {
                    hasSeenPlayer = false;
                    currentState = EnemyState.Returning;
                    StartCoroutine(ReturnToPatrol());
                }
                else if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown)
                {
                    currentState = EnemyState.Attacking;
                    StartCoroutine(PrepareAttack());
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case EnemyState.Attacking:
                if (timeSinceLastAttack >= attackCooldown)
                {
                    currentState = EnemyState.Chasing;
                    ChasePlayer();
                }
                break;
        }
    }

    private void ChasePlayer()
    {
        if (currentState == EnemyState.Chasing && player != null)
        {
            float distanceFromStart = Vector3.Distance(transform.position, chaseStartPoint);
            if (distanceFromStart > maxChaseDistance)
            {
                currentState = EnemyState.Returning;
                StartCoroutine(ReturnToPatrol());
                return;
            }
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
    }

    private IEnumerator PrepareAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Attack();
        }
        else
        {
            currentState = EnemyState.Chasing;
            ChasePlayer();
        }
    }

    private void Attack()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange && timeSinceLastAttack >= attackCooldown)
        {
            var health = player.GetComponent<HealthPlayerController>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
                Debug.Log("Enemy attacked player!");
            }
            else
            {
                Debug.Log("No HealthPlayerController found on player!");
            }
            timeSinceLastAttack = 0f;
        }
        currentState = EnemyState.Chasing;
    }
}
