using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeSelf) return; // Если объект деактивирован, игнорируем урон

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        GameManager.Instance.EnemyKilled(this); // Сообщаем GameManager о смерти
        gameObject.SetActive(false); // Деактивируем объект
    }

    // Сделаем метод Respawn() виртуальным, чтобы его можно было переопределить
    public virtual void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        gameObject.SetActive(true); // Активируем объект

        // Убедимся, что NavMeshAgent активен и его параметры корректны
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 3.5f; // Устанавливаем нормальную скорость
        }
    }


    public IEnumerator GradualHeal()
    {
        // Задержка перед восстановлением
        yield return new WaitForSeconds(1f);

        // Восстанавливаем здоровье до максимума
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
}
