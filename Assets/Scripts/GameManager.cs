using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [SerializeField] private int poolSize;
    [SerializeField] private List<WayPoint> startPoints;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float experiencePerKill = 100f;
    [SerializeField] private float respawnDelay = 15f;
    [SerializeField] private float spawnDelay = 1f;

    private ObjectPool<EnemyBase> enemyPool;
    private PlayerStats playerStats;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    private void Start()
    {
        enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, poolSize, transform);
        StartCoroutine(SpawnEnemiesSequentially());
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (playerStats)
            playerStats.GainExperience(experiencePerKill);
        
        EnemyBase eb = enemy.GetComponent<EnemyBase>();
        if (eb != null)
            eb.OnDeath -= EnemyKilled;
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        while (enemyPool.InactiveCount == 0)
            yield return null;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        EnemyBase enemy = enemyPool.Get();
        enemy.SetPool(enemyPool);
        enemy.transform.position = respawnPoint.position;
        NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.Warp(respawnPoint.position);
        WayPoint start = startPoints[Random.Range(0, startPoints.Count)];
        List<WayPoint> wps = new List<WayPoint>(start.WayPoints);
        ShuffleList(wps);
        EnemyPathFollower follower = enemy.GetComponent<EnemyPathFollower>();
        follower.SetWaypoints(wps.ToArray());
        follower.ResumePatrol();
        enemy.OnDeath += EnemyKilled;
    }

    private void ShuffleList(List<WayPoint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            WayPoint temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator SpawnEnemiesSequentially()
    {
        foreach (var prefab in enemyPrefabs)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public int GetPlayerLevel()
    {
        if (playerStats != null)
            return playerStats.level;
        return 1;
    }
}