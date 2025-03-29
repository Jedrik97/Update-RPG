using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    private int weaponDamage = 25;
    private float deactivateWeaponTime = 3f;

    [SerializeField] private PlayerStats playerStats;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
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

        weaponDamage = 25 + (int)(playerStats.strength * 10);
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