using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int weaponDamage = 25;
    private float deactivateweapon = 3;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    public void EnableCollider(bool enable)
    {
        if (weaponCollider)
        {
            weaponCollider.enabled = enable;
        }
        if (enable)
        {
            Invoke(nameof(DisableCollider), deactivateweapon); // Запускаем таймер на деактивацию
        }
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