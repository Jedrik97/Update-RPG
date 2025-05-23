using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMeleeAI : EnemyBase
{
    [Header("Components & Prefabs")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private EnemyPathFollower pathFollower;

    [Header("Movement & Detection")]
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float maxChaseDistance = 25f;
    [SerializeField] private float attackRange = 3f;

    [Header("Death Settings")]
    [Tooltip("Time in seconds the archer remains dead before deactivating")]
    [SerializeField] private float deathDuration = 5f;
    
    [SerializeField] private UnityEvent onWeaponActivate;
    [SerializeField] private UnityEvent onWeaponDeactivate;
    private NavMeshAgent agent;
    private Transform player;

    private bool hasSeenPlayer;
    private Vector3 chaseStartPoint;

    private enum EnemyState { Patrolling, Chasing, Attacking, Returning, Dead }
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();
        pathFollower = GetComponent<EnemyPathFollower>();

        if (fieldOfView != null)
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
        
        OnHealthChanged += HandleDamageInterrupt;
        OnDeath += HandleDeath;

        pathFollower?.ResumePatrol();
        currentState = EnemyState.Patrolling;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
    }

    private void OnDisable()
    {
        if (fieldOfView != null)
            fieldOfView.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;

        OnHealthChanged -= HandleDamageInterrupt;
        OnDeath -= HandleDeath;
    }

    public void EnableCollider()
    {
        onWeaponActivate?.Invoke();
    }
    public void DisableCollider()
    {
        onWeaponDeactivate?.Invoke();
    }
    private void HandlePlayerVisibilityChanged(bool isVisible)
    {
        if (currentState == EnemyState.Dead)
            return;

        if (isVisible)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                chaseStartPoint = transform.position;
            }
            player = fieldOfView.Player;
            currentState = EnemyState.Chasing;
        }
        else
        {
            hasSeenPlayer = false;
            currentState = EnemyState.Returning;
            agent.isStopped = false;
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead)
            return;

        UpdateWalkingAnimation();

        if (player == null && fieldOfView != null)
            player = fieldOfView.Player;
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (distToPlayer <= maxChaseDistance)
                    currentState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
                if (distFromStart > maxChaseDistance)
                {
                    currentState = EnemyState.Returning;
                    agent.isStopped = false;
                    agent.SetDestination(chaseStartPoint);
                }
                else if (distToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    animator.SetBool("IsWalking", false);
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    RotateTowardsPlayer();
                }
                break;

            case EnemyState.Attacking:
                if (distToPlayer <= attackRange)
                {
                    animator.SetBool("IsAttacking", true);
                    agent.isStopped = true;
                    RotateTowardsPlayer();
                }
                break;

            case EnemyState.Returning:
                agent.isStopped = false;
                agent.SetDestination(chaseStartPoint);
                if (distFromStart <= 1f)
                {
                    agent.isStopped = true;
                    currentState = EnemyState.Patrolling;
                    ReturnHeal();
                }
                break;
        }
    }

    private void UpdateWalkingAnimation()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }
    

    private void RotateTowardsPlayer()
    {
        if (player == null)
            return;
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );
    }
    
    private void HandleDamageInterrupt(float newHealth)
    {
        if (currentState == EnemyState.Attacking)
        {
            currentState = EnemyState.Chasing;
            animator.SetBool("IsAttacking", false);
            agent.isStopped = false;
        }
    }

    private void HandleDeath(GameObject obj)
    {
        currentState = EnemyState.Dead;
        agent.isStopped = true;
        agent.updateRotation = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
        animator.SetTrigger("Die");

        pathFollower?.StopPatrol();

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDuration);
        gameObject.SetActive(false);
    }
}

