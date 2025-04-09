using UnityEngine;
using Zenject;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
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
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        OnHealthChanged?.Invoke(currentHealth);
        if (gameManager != null)
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
        Debug.Log("TakeDamage EnemyBase");
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
        OnDeath?.Invoke(gameObject);
        if (_pool)
        {
            _pool.ReturnToPool(this);
        }   
    }
}
