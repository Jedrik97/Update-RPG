using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMeleeAI : EnemyBase
{
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private EnemyPathFollower pathFollower;

    private NavMeshAgent agent;
    private bool isPreparingAttack = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool hasSeenPlayer = false;
    private Vector3 chaseStartPoint;
    private Transform player;

    private bool isPatrolling = true;

    public float attackRange = 2f;
    public float chaseSpeed = 3.5f;
    public float attackDelay = 1f;
    public float attackSpeed = 1.5f;

    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    
    protected override void OnEnable()
    {
        
        base.OnEnable();
        isPreparingAttack = false;
        isAttacking = false;
        isReturning = false;
        hasSeenPlayer = false;
        isPatrolling = true;
        agent = GetComponent<NavMeshAgent>();

        if (fieldOfView != null)
        {
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
        }
        pathFollower.ReturnToPatrol();
        Debug.Log("дошёл до патруля в мили");
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
        if (isVisible && !isReturning)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                chaseStartPoint = transform.position;
            }

            player = fieldOfView.Player;
                pathFollower.StopPatrol(); // Останавливаем патрулирование
        }
        else if (!isReturning)
        {
            StartCoroutine(CheckReturnCondition());
        }
    }

    private void Update()
    {
        if (player == null || isReturning) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        if (isPatrolling && distanceToPlayer <= 15f)
        {
            StartCoroutine(CheckReturnCondition());
        }

        if (distanceFromStart > 15f)
        {
            ReturnToPatrolAfterDelay();
        }
        else if (distanceToPlayer > attackRange && hasSeenPlayer)
        {
            ChasePlayer();
            Debug.Log("тут3");
        }
        else if (!isPreparingAttack && !isAttacking  && hasSeenPlayer)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    private IEnumerator CheckReturnCondition()
    {
        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, chaseStartPoint) > 15f)
        {
           ReturnToPatrolAfterDelay();
        }
    }

    private void ReturnToPatrolAfterDelay()
    {
        isReturning = true;
        isPreparingAttack = false;
        isAttacking = false;
        agent.isStopped = false;
        
        pathFollower.ReturnToPatrol();
        isReturning = false;
        hasSeenPlayer = false;
        isPatrolling = true;
    }

    private void ChasePlayer()
    {
        Debug.Log("Пытается атаковать");
        if (!isAttacking && !isPreparingAttack && player != null && !isReturning)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(player.position);
            isPatrolling = false;
        }
    }

    private IEnumerator PrepareAttack()
    {
        isPreparingAttack = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(attackDelay);

        if (player != null && !isReturning)
        {
            Attack();
            Debug.Log("тут4");
        }
        else
        {
            isPreparingAttack = false;
            ChasePlayer();
            Debug.Log("тут2");
        }
    }

    private void Attack()
    {
        isPreparingAttack = false;
        isAttacking = true;
        agent.speed = attackSpeed;
        

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange && !isReturning)
        {
            player.GetComponent<HealthPlayerController>()?.TakeDamage(attackDamage);
        }

        isAttacking = false;
        ChasePlayer();
        Debug.Log("тут1");
    }
    
}
