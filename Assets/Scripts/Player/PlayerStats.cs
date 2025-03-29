using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 1000f;

    private float baseExp = 1000f;

    public float strength = 10f;
    public float stamina = 10f;
    public float intelligence = 10f;
    public float wisdom = 10f;

    public int availableStatPoints = 0;

    public float health = 10f; 

    public void GainExperience(float amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel += baseExp * level;

        strength += 1f;
        stamina += 1f;
        intelligence += 1f;
        wisdom += 1f;

        availableStatPoints++;
        
        health = GetMaxHealth();
        
    }
    
    public float GetMaxHealth()
    {
        return health + (stamina * 10f);
    }

    public void SpendStatPoint(string stat)
    {
        if (availableStatPoints > 0)
        {
            switch (stat)
            {
                case "Strength":
                    strength += 1f;
                    break;
                case "Agility":
                    stamina += 1f;
                    break;
                case "Intelligence":
                    intelligence += 1f;
                    break;
                case "Wisdom":
                    wisdom += 1f;
                    break;
            }

            availableStatPoints--; 
            Debug.Log($"Потрачено 1 очко на {stat}. Осталось {availableStatPoints} очков.");
        }
        else
        {
            Debug.Log("Нет доступных очков для распределения.");
        }
    }
}
