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

    [Header("Player Death UI Settings")] 
    [SerializeField] private GameObject deathUI;

    [SerializeField] private float slowPauseDuration = 1f;
    [SerializeField] private float uiFadeDuration = 0.5f;

    [Header("Win UI Settings")] 
    [SerializeField] private GameObject winPanel;

    [SerializeField] private float winUIPersistTime = 30f;

    [Header("Boss Objective UI")] 
    [SerializeField] private GameObject bossObjectivePanel;

    [SerializeField] private int bossObjectiveSortingOrder = 100;

    private ObjectPool<EnemyBase> enemyPool;
    private ObjectPool<EnemyBase> bossPool;

    private PlayerStats playerStats;
    private PlayerInventory playerInventory;

    private bool bossSpawned = false;
    private bool bossDefeated = false;

    private DeathMenuController _deathMenuController;

    private bool objectiveHiddenManually = false;

    [Inject]
    public void Construct(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        this.playerStats = playerStats;
        this.playerInventory = playerInventory;
    }

    public bool BossDefeated => bossDefeated;

    private void Start()
    {
        enemyPool = new ObjectPool<EnemyBase>(enemyPrefabs, poolSize, transform);
        bossPool = new ObjectPool<EnemyBase>(new List<EnemyBase> { bossPrefab }, 0, transform);
        StartCoroutine(SpawnEnemiesSequentially());


        if (deathUI != null)
        {
            _deathMenuController = deathUI.GetComponent<DeathMenuController>();
        }


        if (bossObjectivePanel != null)
        {
            bossObjectivePanel.SetActive(!bossDefeated);
            if (bossObjectivePanel.TryGetComponent(out Canvas objectiveCanvas))
            {
                objectiveCanvas.sortingOrder = bossObjectiveSortingOrder;
            }
        }
        else
        {
            Debug.LogWarning("BossObjectivePanel");
        }
    }

    private void Update()
    {
        if (!bossSpawned && playerStats != null && playerStats.level >= 5)
        {
            bossSpawned = true;
            ClearAllEnemies();
            SpawnBoss();
        }


        if (!bossDefeated && !objectiveHiddenManually && bossObjectivePanel != null && Input.GetKeyDown(KeyCode.Z))
        {
            bossObjectivePanel.SetActive(false);
            objectiveHiddenManually = true;
        }

        if (!bossDefeated && bossObjectivePanel != null && Input.GetKeyDown(KeyCode.Z))
        {
            bool isActive = bossObjectivePanel.activeSelf;
            bossObjectivePanel.SetActive(!isActive);
        }
    }

    private void ClearAllEnemies()
    {
        EnemyBase[] activeEnemies = enemyPool.GetActiveObjects();
        foreach (EnemyBase eb in activeEnemies)
        {
            if (eb == bossPrefab || eb.CompareTag("Boss")) continue;
            eb.OnDeath -= EnemyKilled;
            eb.TakeDamage(20000f);
        }
    }

    public void EnemyKilled(GameObject enemy)
    {
        playerStats?.GainExperience(experiencePerKill);
        playerInventory?.AddGold(1);

        EnemyBase eb = enemy.GetComponent<EnemyBase>();
        if (eb != null)
            eb.OnDeath -= EnemyKilled;

        if (eb == bossPrefab)
        {
            bossDefeated = true;
            OnBossDefeated();
            return;
        }

        if (!bossSpawned)
            StartCoroutine(DelayedSpawn());
    }

    private void OnBossDefeated()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            StartCoroutine(HideWinPanelRoutine());
        }

        if (bossObjectivePanel != null)
        {
            bossObjectivePanel.SetActive(false);
        }
    }

    private IEnumerator HideWinPanelRoutine()
    {
        float elapsed = 0f;
        while (elapsed < winUIPersistTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void SetBossDefeatedFromSave()
    {
        bossDefeated = true;
        bossSpawned = true;
        OnBossDefeated();
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (bossSpawned) yield break;
        while (enemyPool.InactiveCount == 0) yield return null;
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (bossSpawned) return;

        EnemyBase enemy = enemyPool.Get();
        enemy.SetPool(enemyPool);

        enemy.gameObject.SetActive(false);
        Vector3 spawnPos = respawnPoint.position;
        if (NavMesh.SamplePosition(respawnPoint.position, out var hit, 1f, NavMesh.AllAreas))
            spawnPos = hit.position;

        enemy.transform.position = spawnPos;
        var nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.enabled = true;
            nav.Warp(spawnPos);
        }

        enemy.gameObject.SetActive(true);
        StartCoroutine(ContinueSpawnAfterNavMesh(enemy));
    }

    private IEnumerator ContinueSpawnAfterNavMesh(EnemyBase enemy)
    {
        var nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null)
            yield return new WaitUntil(() => nav.isOnNavMesh);

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
        if (NavMesh.SamplePosition(bossSpawnPoint.position, out var hit, 1f, NavMesh.AllAreas))
            bossPos = hit.position;

        boss.transform.position = bossPos;
        var nav = boss.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.enabled = true;
            nav.Warp(bossPos);
        }

        boss.gameObject.tag = "Boss";
        boss.OnDeath += EnemyKilled;
    }

    private void ShuffleList(List<WayPoint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private IEnumerator SpawnEnemiesSequentially()
    {
        foreach (var prefab in enemyPrefabs)
        {
            if (bossSpawned) yield break;
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public int GetPlayerLevel() => playerStats != null ? playerStats.level : 1;


    public void ShowDeathUI()
    {
        StartCoroutine(SlowPauseAndDisplayUI());
    }

    private IEnumerator SlowPauseAndDisplayUI()
    {
        float elapsed = 0f;
        while (elapsed < slowPauseDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slowPauseDuration);
            Time.timeScale = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        Time.timeScale = 0f;


        if (_deathMenuController != null)
        {
            _deathMenuController.ShowDeathMenu();
        }
    }

    public void ShowBossObjectivePanelIfNeeded()
    {
        if (!bossDefeated && bossObjectivePanel)
        {
            bossObjectivePanel.SetActive(true);
            objectiveHiddenManually = false;
        }
    }


    public bool IsDeathMenuVisible
    {
        get
        {
            if (_deathMenuController != null && _deathMenuController.deathPanel != null)
                return _deathMenuController.deathPanel.activeInHierarchy;

            return false;
        }
    }
}