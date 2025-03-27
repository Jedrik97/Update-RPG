using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs; // Префабы врагов
    private Queue<GameObject> inactiveEnemies = new Queue<GameObject>(); // Очередь для неактивных врагов

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        // Добавляем врага в очередь для повторного использования
        inactiveEnemies.Enqueue(enemy);
    }

    public void EnemyKilled(EnemyBase enemy)
    {
        if (enemy != null)
        {
            // Запускаем респаун врага
            StartCoroutine(RespawnEnemy(enemy));
        }
    }

    private IEnumerator RespawnEnemy(EnemyBase enemy)
    {
        // Задержка респауна
        yield return new WaitForSeconds(10f);  // Например, 30 секунд

        // Проверяем, является ли враг экземпляром EnemyMeleeAI
        if (enemy is EnemyMeleeAI)
        {
            // Вызов респауна для врага, если это EnemyMeleeAI
            (enemy as EnemyMeleeAI).Respawn();
        }
        else
        {
            // Вызов респауна для EnemyBase
            enemy.Respawn();
        }
    }

}
    