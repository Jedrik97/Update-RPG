using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int damage = 20;
    [SerializeField] private float activeDuration = 0.5f;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    // Вызывать через UnityEvent onWeaponActivate в EnemyMeleeAI
    public void EnableCollider()
    {
        if (weaponCollider == null)
        {
            Debug.LogWarning("EnemyWeapon: weaponCollider не назначен!");
            return;
        }
        weaponCollider.enabled = true;
        Invoke(nameof(DisableCollider), activeDuration);
    }

    private void DisableCollider()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var health = other.GetComponent<HealthPlayerController>();
        if (health != null)
            health.TakeDamage(damage);
    }
}