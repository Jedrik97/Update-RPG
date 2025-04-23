using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    
    private GameManager gameManager;

    public event System.Action<float> OnHealthChanged;
    public event System.Action<GameObject> OnDeath;

    private ObjectPool<EnemyBase> _pool;

    protected Animator animator;
    
    [Inject]
    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        if (gameManager)
        {
            ApplyLevelBasedStats(gameManager.GetPlayerLevel());
        }
        animator = GetComponent<Animator>();
    }

    public void SetPool(ObjectPool<EnemyBase> pool)
    {
        this._pool = pool;
    }

    public void ApplyLevelBasedStats(int playerLevel)
    {
        attackDamage = 10f + (playerLevel * 5f);
        maxHealth = 100f + (playerLevel * 10f);
    }

    public void ReturnHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeSelf) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > 0f)
        {
            // Запуск анимации получения урона
            if (animator)
                animator.SetTrigger("TakeDamage");
        }
        else
        {
            // Если здоровье упало до нуля или ниже — сразу смерть
            Die();
        }
    }

    private void Die()
    {
        if (animator)
            animator.SetTrigger("Die");

        OnDeath?.Invoke(gameObject);
        StartCoroutine(WaitAndReturnToPool());
    }

    private IEnumerator WaitAndReturnToPool()
    {
        // Ждём перед возвратом в пул, чтобы анимация успела проиграться
        yield return new WaitForSeconds(60f);
        _pool?.ReturnToPool(this);
    }
}
