using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcherAI : EnemyBase
{
    [Header("Components & Prefabs")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private int initialArrowCount = 10;

    [Header("Movement & Detection")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float maxChaseDistance = 15f;
    [SerializeField] private float attackRange = 15f;

    [Header("Attack Timings")]
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float attackCooldown = 3f;

    private Animator animator;
    private ObjectPool<Arrow> arrowPool;
    private NavMeshAgent agent;
    private Transform player;

    private bool hasSeenPlayer = false;
    private Vector3 chaseStartPoint;
    private float timeSinceLastShot;

    private enum EnemyState { Patrolling, Chasing, Attacking, Returning }
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        arrowPool = new ObjectPool<Arrow>(new List<Arrow> { arrowPrefab }, initialArrowCount, transform);

        if (fieldOfView)
        {
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
        }
        
        timeSinceLastShot = attackCooldown;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(PerformShot));
        
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

            currentState = EnemyState.Chasing;

            // Если игрок в зоне поражения, запускаем корутину для выстрела
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                if (shootCoroutine == null)
                {
                    shootCoroutine = StartCoroutine(ShootAtPlayer());
                }
            }
        }
        else
        {
            hasSeenPlayer = false;
            currentState = EnemyState.Returning;
            agent.isStopped = false;

            // Если игрок выходит из зоны поражения, останавливаем корутину
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
        }
    }


    private void FixedUpdate()
{
    timeSinceLastShot += Time.fixedDeltaTime;
    if (player == null)
        player = fieldOfView?.Player;
    if (player == null) return;

    float distToPlayer = Vector3.Distance(transform.position, player.position);
    float distFromStart = Vector3.Distance(transform.position, chaseStartPoint);

    // Устанавливаем максимальную дистанцию для преследования
    float maxChaseDistance = 20f;

    switch (currentState)
    {
        case EnemyState.Patrolling:
            break;

        case EnemyState.Chasing:
            if (distToPlayer > attackRange && distToPlayer <= maxChaseDistance && timeSinceLastShot >= attackCooldown)
            {
                // Если игрок в пределах зоны поражения, стрелок останавливается и атакует
                currentState = EnemyState.Attacking;
                agent.isStopped = true;
                Invoke(nameof(PerformShot), attackDelay);
            }
            else if (distToPlayer > maxChaseDistance)
            {
                // Если игрок убежал за пределы 20 единиц, стрелок продолжает следить за ним
                if (distToPlayer <= attackRange)
                {
                    // Пауза между выстрелами
                    if (timeSinceLastShot >= attackCooldown)
                    {
                        Shoot();
                        timeSinceLastShot = 0f;
                    }
                }
                else
                {
                    // Стрелок перестает преследовать игрока, но продолжает следить
                    agent.isStopped = true;
                    agent.SetDestination(transform.position); // Останавливаем движение
                    currentState = EnemyState.Chasing; // Он просто продолжает следить
                }
            }
            break;

        case EnemyState.Attacking:
            if (distToPlayer > attackRange)
            {
                currentState = EnemyState.Chasing;
                agent.isStopped = false;
            }
            else if (timeSinceLastShot >= attackCooldown && !IsInvoking(nameof(PerformShot)))
            {
                agent.isStopped = true;
                Invoke(nameof(PerformShot), attackDelay);
            }
            break;

        case EnemyState.Returning:
            if (distFromStart <= 1f)
            {
                agent.isStopped = true;
                currentState = EnemyState.Patrolling;
                ReturnHeal();
            }
            break;
    }
}


    private void PerformShot()
    {
        if (player && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
            timeSinceLastShot = 0f;
        }
        if (hasSeenPlayer)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            currentState = EnemyState.Returning;
            agent.isStopped = false;
            agent.SetDestination(chaseStartPoint);
        }
    }

    private Coroutine shootCoroutine;

    private IEnumerator ShootAtPlayer()
    {
        while (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Shoot();  // Выстрел
            yield return new WaitForSeconds(attackCooldown);  // Пауза между выстрелами
        }
    }

    private void Shoot()
    {
        Debug.Log("выстрелил");
        if (fieldOfView.Player)
        {
            Arrow arrow = arrowPool.Get();

            // Получаем позицию игрока, но учитываем высоту (например, 0.7 по Y)
            Vector3 targetPosition = new Vector3(fieldOfView.Player.position.x, fieldOfView.Player.position.y + 0.7f, fieldOfView.Player.position.z);

            // Устанавливаем позицию и поворот стрелы
            arrow.transform.position = arrowSpawnPoint.position;
            arrow.transform.rotation = arrowSpawnPoint.rotation;

            // Направляем стрелу по вектору, направленному на цель (игрока)
            Vector3 direction = targetPosition - arrow.transform.position;
            arrow.Initialize(direction, arrowPool);
        }
    }

    
    
}

