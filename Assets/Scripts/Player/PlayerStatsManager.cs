using UnityEngine;
using Zenject;

public interface IPlayerStatsManager
{
    void GainExperience(float amount);
    void LevelUp();
    void SpendStatPoint(string stat);
}

public class PlayerStatsManager : IPlayerStatsManager
{
    [Inject] private PlayerStats playerStats;

    public PlayerStatsManager(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void GainExperience(float amount)
    {
        playerStats.currentExp += amount;

        while (playerStats.currentExp >= playerStats.expToNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        playerStats.level++;
        playerStats.currentExp -= playerStats.expToNextLevel;
        playerStats.expToNextLevel += 1000 * playerStats.level;

        playerStats.strength += 1f;
        playerStats.stamina += 1f;
        playerStats.intelligence += 1f;
        playerStats.wisdom += 1f;

        playerStats.availableStatPoints++;
        playerStats.health = playerStats.stamina * 10;
    }

    public void SpendStatPoint(string stat)
    {
        if (playerStats.availableStatPoints > 0)
        {
            switch (stat)
            {
                case "Strength":
                    playerStats.strength += 1f;
                    break;
                case "Stamina":
                    playerStats.stamina += 1f;
                    break;
                case "Intelligence":
                    playerStats.intelligence += 1f;
                    break;
                case "Wisdom":
                    playerStats.wisdom += 1f;
                    break;
            }
            playerStats.availableStatPoints--;
        }
        else
        {
            Debug.Log("No stat points available.");
        }
    }
}