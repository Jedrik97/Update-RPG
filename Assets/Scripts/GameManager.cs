using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public class GameManager : MonoBehaviour
{

    private PlayerStats playerStats;
    private EnemyBase enemy = new EnemyBase();

    public float experiencePerKill = 100f;
    public float respawnDelay = 15f;

    
    private void OnEnable()
    {
        enemy.OnDeath += EnemyKilled;
    }

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }
    
    
    
    public void EnemyKilled(GameObject Enemy)
    {
        Debug.Log("+++");
        if (playerStats)
        {
            playerStats.currentExp += experiencePerKill;
            while (playerStats.currentExp >= playerStats.expToNextLevel)
            {
                LevelUp();
            }
        }
        StartCoroutine(RespawnEnemy());
    }

    private void LevelUp()
    {
        playerStats.LevelUp();
        /*Debug.Log($"Player leveled up! New level: {playerStats.level}");*/
    }

    private IEnumerator RespawnEnemy()
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
    private void OnDisable()
    {
        enemy.OnDeath -= EnemyKilled;
    }
}