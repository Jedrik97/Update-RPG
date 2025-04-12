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

    public delegate void HealDelegate();
    public event System.Action<float> OnHealthChanged;
    public event System.Action<GameObject> OnDeath;
    public HealDelegate OnHealRequested;

    private ObjectPool<EnemyBase> _pool;

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
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Die Enemy");
        OnDeath?.Invoke(gameObject);
        _pool?.ReturnToPool(this);
        
    }
}