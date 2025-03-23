using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMeleeAI : EnemyBase
{
    [SerializeField] private FieldOfView fieldOfView;
    
    private NavMeshAgent agent;
    private bool isPreparingAttack = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool hasSeenPlayer = false;
    private Vector3 chaseStartPoint;
    private Transform player;

    public delegate void StopPatrolDelegate();
    public event StopPatrolDelegate OnStopPatrol;
    public delegate void ReturnToPatrolDelegate();
    public event ReturnToPatrolDelegate OnReturnToPatrol;

    // Добавляем недостающие переменные
    public float attackRange = 2f;  // Радиус атаки
    public float chaseSpeed = 3.5f; // Скорость преследования
    public float attackDelay = 1f;  // Задержка перед атакой
    public float attackSpeed = 1.5f; // Скорость атаки

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();

        if (fieldOfView != null)
        {
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
        }
    }

    private void OnDestroy()
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
                chaseStartPoint = transform.position; // Запоминаем точку начала погони
            }

            player = fieldOfView.Player; // Используем игрока из FOV
            OnStopPatrol?.Invoke();
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

        if (distanceFromStart > 15f)
        {
            StartCoroutine(ReturnToPatrolAfterDelay());
        }
        else if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else if (!isPreparingAttack && !isAttacking)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    private IEnumerator CheckReturnCondition()
    {
        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, chaseStartPoint) > 15f)
        {
            StartCoroutine(ReturnToPatrolAfterDelay());
        }
    }

    private IEnumerator ReturnToPatrolAfterDelay()
    {
        isReturning = true; // Блокируем логику атаки и преследования
        isPreparingAttack = false;
        isAttacking = false;
        agent.isStopped = false;
        agent.SetDestination(chaseStartPoint);
        OnReturnToPatrol?.Invoke();
        StartCoroutine(GradualHeal()); // Начинаем постепенное восстановление здоровья

        while (Vector3.Distance(transform.position, chaseStartPoint) > 1f)
        {
            yield return null;
        }

        isReturning = false; // Разрешаем снова реагировать на игрока
        hasSeenPlayer = false; // Враг забывает о старой цели
    }

    private void ChasePlayer()
    {
        if (!isAttacking && !isPreparingAttack && player != null && !isReturning)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private IEnumerator PrepareAttack()
    {
        isPreparingAttack = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(attackDelay);

        if (player != null && !isReturning)
        {
            StartCoroutine(Attack());
        }
        else
        {
            isPreparingAttack = false;
            ChasePlayer();
        }
    }

    private IEnumerator Attack()
    {
        isPreparingAttack = false;
        isAttacking = true;
        agent.speed = attackSpeed;

        yield return new WaitForSeconds(0.5f);

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange && !isReturning)
        {
            player.GetComponent<HealthPlayerController>()?.TakeDamage(attackDamage);
        }

        isAttacking = false;
        ChasePlayer();
    }
}
