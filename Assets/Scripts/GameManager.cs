using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [SerializeField] private int _poolSize;
    [SerializeField] private List<WayPoint> startPoints;
    [SerializeField] private Transform respawnPoint;

    private ObjectPool<EnemyBase> _enemyPool;
    private PlayerStats _playerStats;

    public float experiencePerKill = 100f;
    public float respawnDelay = 15f;
    public float spawnDelay = 1f; 

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this._playerStats = playerStats;
    }

    private void Start()
    {
        _enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, _poolSize, transform);
        StartCoroutine(SpawnEnemiesSequentially());
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (_playerStats)
        {
            _playerStats.currentExp += experiencePerKill;
            while (_playerStats.currentExp >= _playerStats.expToNextLevel)
            {
                LevelUp();
            }
        }
        
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase)
        {
            enemyBase.OnDeath -= EnemyKilled;
        }

        StartCoroutine(RespawnEnemy());
    }

    private void LevelUp()
    {
        _playerStats.LevelUp();
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(respawnDelay);
        EnemySpawn();
    }
    
    public void EnemySpawn()
    {
        EnemyBase enemy = _enemyPool.Get();
        enemy.SetPool(_enemyPool);
        
        enemy.transform.position = respawnPoint.position;
        
        WayPoint startPoint = startPoints[Random.Range(0, startPoints.Count)];
        
        List<WayPoint> shuffledWayPoints = new List<WayPoint>(startPoint.WayPoints);
        ShuffleList(shuffledWayPoints);
        
        enemy.GetComponent<EnemyPathFollower>().SetWaypoints(shuffledWayPoints.ToArray());
        
        enemy.OnDeath += EnemyKilled;
    }
    
    private void ShuffleList(List<WayPoint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            WayPoint temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public int GetPlayerLevel()
    {
        if (_playerStats)
        {
            return _playerStats.level;  
        }
        return 1; 
    }
    private IEnumerator SpawnEnemiesSequentially()
    {
        foreach (var enemyPrefab in enemyPrefabs)
        {
            EnemySpawn();
            yield return new WaitForSeconds(spawnDelay); 
        }
    }
}
