using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMeleeAI : EnemyBase
{
    [Header("Components & Prefabs")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private EnemyPathFollower pathFollower;
    [SerializeField] private Animator animator;
    [Tooltip("Invoke this event (e.g. EnemyWeapon.EnableCollider) at the attack moment")]
    [SerializeField] private UnityEvent onWeaponActivate;

    [Header("Stats & Timing")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float maxChaseDistance = 15f;

    private NavMeshAgent agent;
    private Transform player;
    private bool hasSeenPlayer = false;
    private Vector3 chaseStartPoint;
    private float timeSinceLastAttack = Mathf.Infinity;

    private enum EnemyState { Patrolling, Chasing, Attacking, Returning }
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        agent.isStopped = false;
        hasSeenPlayer = false;
        timeSinceLastAttack = attackCooldown;

        if (fieldOfView)
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;

        // Начинаем патруль
        pathFollower?.ResumePatrol();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(PerformAttack));
        if (fieldOfView)
            fieldOfView.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;
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
            if (player == null) return;

            pathFollower?.StopPatrol();
            currentState = EnemyState.Chasing;
        }
        else
        {
            hasSeenPlayer = false;
            currentState = EnemyState.Returning;
            pathFollower?.StopPatrol();
            agent.isStopped = false;
            agent.SetDestination(chaseStartPoint);
            animator.SetBool("IsAttacking", false);
        }
    }

    private void FixedUpdate()
    {
        timeSinceLastAttack += Time.fixedDeltaTime;

        if (player == null)
            player = fieldOfView?.Player;
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        UpdateMovementAnimation();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                pathFollower?.ResumePatrol();
                animator.SetBool("IsWalking", true);
                if (distToPlayer <= maxChaseDistance && hasSeenPlayer)
                {
                    currentState = EnemyState.Chasing;
                    pathFollower?.StopPatrol();
                }
                break;

            case EnemyState.Chasing:
                if (distFromStart > maxChaseDistance)
                {
                    currentState = EnemyState.Returning;
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsAttacking", false);
                }
                else if (distToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown)
                {
                    currentState = EnemyState.Attacking;
                    animator.SetBool("IsAttacking", true);
                    Invoke(nameof(PerformAttack), attackDelay);
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                }
                break;

            case EnemyState.Attacking:
                if (timeSinceLastAttack >= attackCooldown)
                {
                    currentState = EnemyState.Chasing;
                    animator.SetBool("IsAttacking", false);
                }
                break;

            case EnemyState.Returning:
                agent.isStopped = false;
                agent.SetDestination(chaseStartPoint);
                if (distFromStart <= 1f)
                {
                    agent.isStopped = true;
                    currentState = EnemyState.Patrolling;
                    animator.SetBool("IsAttacking", false);
                    pathFollower?.ResumePatrol();
                    ReturnHeal();
                }
                break;
        }
    }

    private void UpdateMovementAnimation()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    private void PerformAttack()
    {
        agent.isStopped = true;
        timeSinceLastAttack = 0f;
        // Триггерим событие активации оружия (EnableCollider в EnemyWeapon)
        onWeaponActivate?.Invoke();
    }
}