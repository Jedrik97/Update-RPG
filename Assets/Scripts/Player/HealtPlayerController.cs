using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthPlayerController : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    
    public TextMeshProUGUI healthText;
    public Image healthOrb;

    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        
        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void UpdateHealthBar()
    {
        if (healthOrb)
        {
            healthOrb.fillAmount = currentHealth / maxHealth;
        }

        if (healthText)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
    }
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}