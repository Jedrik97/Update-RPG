using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


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
        _enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, 10, transform);
        SpawnEnemies();
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
        
        /*if (enemyBase)
        {
            enemyBase.OnDeath += EnemyKilled;
        }*/
        
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
    private void SpawnEnemies()
    {
        EnemySpawn();
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
    /*private void OnDisable()
    {
        EnemyBase.OnDeath -= EnemyKilled;
    }*/
}

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _poolQueue;
    private List<T> _prefabs;
    private Transform _parent;

    public ObjectPool(List<T> prefabs, int initialSizePerPrefab, Transform parent = null)
    {
        this._prefabs = prefabs;
        this._parent = parent;
        this._poolQueue = new Queue<T>();

        Shuffle(prefabs);

        foreach (var prefab in prefabs)
        {
            for (int i = 0; i < initialSizePerPrefab; i++)
            {
                T obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                _poolQueue.Enqueue(obj);
            }
        }

        
        public T Get()
        {
            T obj;
            if (_poolQueue.Count > 0)
            {
                obj = _poolQueue.Dequeue();
            }
            else
            {
                T prefab = _prefabs[Random.Range(0, _prefabs.Count)];
                obj = Object.Instantiate(prefab, _parent);
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
        }
        
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int randomIndex = Random.Range(i, n);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
    }
}