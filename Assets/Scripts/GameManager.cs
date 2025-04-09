using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [SerializeField] private int _poolSize;
    
    private ObjectPool<EnemyBase> _enemyPool;
    private PlayerStats _playerStats;

    public float experiencePerKill = 100f;
    public float respawnDelay = 15f;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this._playerStats = playerStats;
    }

    private void Start()
    {
        _enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, _poolSize, transform);
        SubscribeToEnemyDeath(); 
    }

    private void OnDisable()
    {
        UnsubscribeFromEnemyDeath();
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

    private void SubscribeToEnemyDeath()
    {
        foreach (var prefab in enemyPrefabs)
        {
            if (prefab)
            {
                prefab.OnDeath += EnemyKilled;
            }
        }
    }

    private void UnsubscribeFromEnemyDeath()
    {
        foreach (var prefab in enemyPrefabs)
        {
            if (prefab)
            {
                prefab.OnDeath -= EnemyKilled;
            }
        }
    }

    public void EnemySpawn()
    {
        EnemyBase enemy = _enemyPool.Get();
        enemy.SetPool(_enemyPool);    
    }

    public int GetPlayerLevel()
    {
        if (_playerStats)
        {
            return _playerStats.level;  
        }
        return 1; 
    }
}
