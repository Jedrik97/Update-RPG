using UnityEngine;
using System.Collections;
using Zenject;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Inject] private PlayerStats playerStats;

    public float experiencePerKill = 100f;
    public float respawnDelay = 15f;

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

    public void EnemyKilled(EnemyBase enemy)
    {
        if (playerStats != null)
        {
            playerStats.currentExp += experiencePerKill;
            while (playerStats.currentExp >= playerStats.expToNextLevel)
            {
                LevelUp();
            }
        }
        StartCoroutine(RespawnEnemy(enemy));
    }

    private void LevelUp()
    {
        playerStats.LevelUp();
        Debug.Log($"Player leveled up! New level: {playerStats.level}");
    }

    private IEnumerator RespawnEnemy(EnemyBase enemy)
    {
        yield return new WaitForSeconds(respawnDelay);
        enemy.Respawn();
    }
    public int GetPlayerLevel()
    {
        if (playerStats != null)
        {
            return playerStats.level;  
        }
        return 1; 
    }
}