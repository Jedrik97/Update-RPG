using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float respawnDelay = 15f;
    public PlayerStats playerStats;
    public float experiencePerKill = 100f;

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
        if (enemy != null)
        {
            if (playerStats != null)
            {
                playerStats.GainExperience(experiencePerKill);
            }
            StartCoroutine(RespawnEnemy(enemy));
        }
    }

    private IEnumerator RespawnEnemy(EnemyBase enemy)
    {
        yield return new WaitForSeconds(respawnDelay);
        enemy.Respawn();
    }
}