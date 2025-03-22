using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float attackDamage = 10f;

    [Header("Attack Parameters")]
    public float attackRange = 1.5f;
    public float attackDelay = 0.5f;
    public float chaseSpeed = 3.5f;
    public float attackSpeed = 0f;
    
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;
    
    private Coroutine healingCoroutine;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void StartHealing()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }
        healingCoroutine = StartCoroutine(GradualHeal());
    }

    public IEnumerator GradualHeal()
    {
        float healDuration = 3f;
        float healAmount = maxHealth / healDuration;

        for (float t = 0; t < healDuration; t += Time.deltaTime)
        {
            Heal(healAmount * Time.deltaTime);
            yield return null;
        }
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}