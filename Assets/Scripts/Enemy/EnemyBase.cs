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

    public event System.Action<float> OnHealthChanged;
    

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
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

    private void Die()
    {
        GameManager.Instance.EnemyDied(this);
        Destroy(gameObject);
    }

    public void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        gameObject.SetActive(true);
    }

    public IEnumerator GradualHeal()
    {
        yield return new WaitForSeconds(1f);
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
}