using Zenject;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerStats playerStats;
    
    [SerializeField] private Collider weaponCollider;
    
    private int weaponDamage;
    private float deactivateWeaponTime = 2f;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }
    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }
    
    public void EnableCollider(bool enable)
    {
        if (weaponCollider == null)
        {
            Debug.Log("weaponCollider не назначен!");
            return;
        }

        weaponCollider.enabled = enable;

        if (enable)
        {
            UpdateWeaponDamage();
            Invoke(nameof(DisableCollider), deactivateWeaponTime);
        }
        Debug.Log("EnableCollider Weapon");
    }

    private void UpdateWeaponDamage()
    {
        if (playerStats == null)
        {
            Debug.Log("playerStats не назначен!");
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
        Debug.Log("OnTriggerEnter");
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out EnemyBase enemyBase))
        {
            enemyBase.TakeDamage(weaponDamage);
        }
    }
}