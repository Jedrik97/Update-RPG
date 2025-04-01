using Zenject;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Inject] private PlayerStats playerStats;
    
    [SerializeField] private Collider weaponCollider;
    private int weaponDamage;
    private float deactivateWeaponTime = 3f;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    [Inject]
    private void OnInject()
    {
        if (playerStats == null)
        {
            Debug.LogError("playerStats не назначен!");
        }
        else
        {
            Debug.Log("playerStats инжектирован!");
        }
    }
    public void EnableCollider(bool enable)
    {
        if (weaponCollider == null)
        {
            Debug.LogError("weaponCollider не назначен!");
            return;
        }

        weaponCollider.enabled = enable;

        if (enable)
        {
            UpdateWeaponDamage();
            Invoke(nameof(DisableCollider), deactivateWeaponTime);
        }
    }

    private void UpdateWeaponDamage()
    {
        if (playerStats == null)
        {
            Debug.LogError("playerStats не назначен!");
            return;
        }

        weaponDamage = 25 + (int)(playerStats.strength * 5);
    }

    private void DisableCollider()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out EnemyBase enemyBase))
        {
            enemyBase.TakeDamage(weaponDamage);
        }
    }
}