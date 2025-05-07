using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthPlayerController : MonoBehaviour
{
    public float maxHealth=100;
    private float currentHealth;
    [Header("UI")]
    public Image healthOrb;
    public TextMeshProUGUI healthText;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float d)
    {
        currentHealth = Mathf.Clamp(currentHealth-d,0,maxHealth);
        UpdateHealthBar();
        if (currentHealth<=0) Debug.Log("Player Died");
    }
    public void Heal(float h)
    {
        currentHealth = Mathf.Clamp(currentHealth+h,0,maxHealth);
        UpdateHealthBar();
    }
    public void SetHealth(float h)
    {
        currentHealth = Mathf.Clamp(h,0,maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthOrb)  healthOrb.fillAmount = currentHealth/maxHealth;
        if (healthText) healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
    }

    public float GetCurrentHealth() => currentHealth;
}