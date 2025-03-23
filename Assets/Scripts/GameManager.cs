using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public List<Transform> spawnPoints;

    private Dictionary<EnemyBase, Transform> activeEnemies = new Dictionary<EnemyBase, Transform>();

    [Header("Player")]
    public int playerExperience = 0; 

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

    public void RegisterEnemy(EnemyBase enemy, Transform spawnPoint)
    {
        if (!activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Add(enemy, spawnPoint);
        }
    }

    public void EnemyDied(EnemyBase enemy)
    {
        int experienceGained = 10;
        playerExperience += experienceGained;
        Debug.Log("Player gained " + experienceGained + " experience. Total: " + playerExperience);
        
        if (activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        
        StartCoroutine(RespawnEnemy(enemy));
    }

    private IEnumerator RespawnEnemy(EnemyBase deadEnemy)
    {
        // Задержка перед респавном врага
        yield return new WaitForSeconds(1f);

        // Рандомно выбираем врага и точку спавна
        int enemyIndex = Random.Range(0, enemyPrefabs.Count);
        int spawnPointIndex = Random.Range(0, spawnPoints.Count);

        // Спавним нового врага
        GameObject newEnemyObject = Instantiate(enemyPrefabs[enemyIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);

        // Получаем компонент EnemyBase и регистрируем его в GameManager
        EnemyBase newEnemy = newEnemyObject.GetComponent<EnemyBase>();
        RegisterEnemy(newEnemy, spawnPoints[spawnPointIndex]);
        newEnemy.Respawn(); // Возвращаем врага в действие (если нужно)
    }
}
