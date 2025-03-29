using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public delegate void HealDelegate();
    public HealDelegate OnHealRequested;
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;

    
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void GradualHeal()
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
        OnDeath?.Invoke();
        GameManager.Instance.EnemyKilled(this);
        gameObject.SetActive(false); 
        
    }
    
    public void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        gameObject.SetActive(true); 
    }
    
}
