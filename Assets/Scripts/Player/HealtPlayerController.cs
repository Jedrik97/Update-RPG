using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Zenject;

public class HealthPlayerController : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;

    [Header("UI")]
    public Image healthOrb;
    public TextMeshProUGUI healthText;

    private Coroutine regenCoroutine;
    private PlayerInventory _inventory;

    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    void OnEnable()
    {
        PlayerInput.OnUsePotion += HandleUsePotion;
    }

    void OnDisable()
    {
        PlayerInput.OnUsePotion -= HandleUsePotion;
    }

    private void HandleUsePotion()
    {
        bool used = _inventory.UseHealthPotion(this);
        if (!used)
            Debug.Log("Нет фляжек для использования.");
    }

    public void TakeDamage(float d)
    {
        currentHealth = Mathf.Clamp(currentHealth - d, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0) Debug.Log("Player Died");
    }

    public void Heal(float h)
    {
        currentHealth = Mathf.Clamp(currentHealth + h, 0, maxHealth);
        UpdateHealthBar();
    }

    public void SetHealth(float h)
    {
        currentHealth = Mathf.Clamp(h, 0, maxHealth);
        UpdateHealthBar();
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentHealth < maxHealth)
                Heal(1f);
        }
    }

    void UpdateHealthBar()
    {
        if (healthOrb) healthOrb.fillAmount = currentHealth / maxHealth;
        if (healthText) healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
    }

    public float GetCurrentHealth() => currentHealth;
}