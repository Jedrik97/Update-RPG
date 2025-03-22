using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private int damage;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthPlayerController health = other.GetComponent<HealthPlayerController>();
            health?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}