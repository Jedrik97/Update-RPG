using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Header("Regular Enemies")]
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [SerializeField] private int poolSize;
    [SerializeField] private List<WayPoint> startPoints;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float experiencePerKill = 100f;
    [SerializeField] private float respawnDelay = 15f;
    [SerializeField] private float spawnDelay = 1f;

    [Header("Boss Settings")]
    [SerializeField] private EnemyBase bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    private ObjectPool<EnemyBase> enemyPool;
    private ObjectPool<EnemyBase> bossPool;
    private PlayerStats playerStats;
    private PlayerInventory playerInventory;
    private bool bossSpawned = false;

    [Inject]
    public void Construct(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        this.playerStats = playerStats;
        this.playerInventory = playerInventory;
    }

    private void Start()
    {
        enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, poolSize, transform);
        bossPool = new ObjectPool<EnemyBase>(new List<EnemyBase> { bossPrefab }, 0, transform);
        StartCoroutine(SpawnEnemiesSequentially());
    }

    private void Update()
    {
        if (!bossSpawned && playerStats != null && playerStats.level >= 5)
        {
            bossSpawned = true;
            SpawnBoss();
        }
    }

    public void EnemyKilled(GameObject enemy)
    {
        playerStats?.GainExperience(experiencePerKill);
        playerInventory?.AddGold(1);

        EnemyBase eb = enemy.GetComponent<EnemyBase>();
        if (eb != null) eb.OnDeath -= EnemyKilled;
        if (eb == bossPrefab) return;

        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        while (enemyPool.InactiveCount == 0) yield return null;
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        EnemyBase enemy = enemyPool.Get();
        enemy.SetPool(enemyPool);

        enemy.gameObject.SetActive(false);
        Vector3 spawnPos = respawnPoint.position;
        if (NavMesh.SamplePosition(respawnPoint.position, out var hit, 1f, NavMesh.AllAreas)) spawnPos = hit.position;
        else Debug.LogWarning($"RespawnPoint too far from NavMesh: {respawnPoint.position}");

        enemy.transform.position = spawnPos;
        var nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null) { nav.enabled = true; nav.Warp(spawnPos); }

        enemy.gameObject.SetActive(true);
        StartCoroutine(ContinueSpawnAfterNavMesh(enemy));
    }

    private IEnumerator ContinueSpawnAfterNavMesh(EnemyBase enemy)
    {
        var nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null) yield return new WaitUntil(() => nav.isOnNavMesh);

        WayPoint start = startPoints[Random.Range(0, startPoints.Count)];
        var wps = new List<WayPoint>(start.WayPoints);
        ShuffleList(wps);

        var follower = enemy.GetComponent<EnemyPathFollower>();
        follower.SetWaypoints(wps.ToArray());
        follower.ResumePatrol();
        enemy.OnDeath += EnemyKilled;
    }

    private void SpawnBoss()
    {
        EnemyBase boss = bossPool.Get();
        boss.SetPool(bossPool);

        Vector3 bossPos = bossSpawnPoint.position;
        if (NavMesh.SamplePosition(bossSpawnPoint.position, out var hit, 1f, NavMesh.AllAreas)) bossPos = hit.position;
        else Debug.LogWarning($"BossSpawnPoint too far from NavMesh: {bossSpawnPoint.position}");

        boss.transform.position = bossPos;
        var nav = boss.GetComponent<NavMeshAgent>();
        if (nav != null) { nav.enabled = true; nav.Warp(bossPos); }
        boss.OnDeath += EnemyKilled;
    }

    private void ShuffleList(List<WayPoint> list)
    {
        for (int i = 0; i < list.Count; i++) { int j = Random.Range(i, list.Count); (list[i], list[j]) = (list[j], list[i]); }
    }

    private IEnumerator SpawnEnemiesSequentially()
    {
        foreach (var prefab in enemyPrefabs) { SpawnEnemy(); yield return new WaitForSeconds(spawnDelay); }
    }

    public int GetPlayerLevel() => playerStats != null ? playerStats.level : 1;
}