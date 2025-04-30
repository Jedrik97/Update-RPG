using UnityEngine;

public class EnemyWeaponBoss : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int damage = 20;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }
    
    public void EnableCollider()
    {
        Debug.Log("включился");
        if (weaponCollider == null)
        {
            return;
        }
        weaponCollider.enabled = true;
    }

    public void DisableCollider()
    {
        Debug.Log("выключился");
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