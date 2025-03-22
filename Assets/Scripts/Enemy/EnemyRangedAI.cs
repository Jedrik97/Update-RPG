using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedAI : MonoBehaviour
{
    public float chaseSpeed = 2f;
    public float shootingDistance = 10f;
    public float attackCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int attackDamage = 5;

    private Transform player;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool isChasing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= shootingDistance)
        {
            AttemptAttack();
        }
        else if (isChasing && distance <= 15f)
        {
            ChasePlayer();
        }
    }

    public void SetTarget(Transform target)
    {
        player = target;
        isChasing = (player != null);
    }

    public bool ChasePlayer()
    {
        if (player == null) return false;

        agent.SetDestination(player.position);
        return true;
    }

    private void AttemptAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Invoke(nameof(Shoot), 1.5f);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetDamage(attackDamage);
        }
    }
    public void ResetTarget()
    {
        player = null;
        agent.isStopped = false;
    }

}