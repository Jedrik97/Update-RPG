using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 10f; // Урон врага

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что игрок имеет тег "Player"
        if (other.CompareTag("Player"))
        {
            // Получаем компонент здоровья игрока
            HealthPlayerController playerHealth = other.GetComponent<HealthPlayerController>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(damage); // Наносим урон игроку
            }
        }
    }
}