using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 1000f;

    public float strength = 1f;
    public float stamina = 1f;
    public float intelligence = 1f;
    public float wisdom = 1f;
    
    public int availableStatPoints = 0;

    public float health = 90f;
    
    public float GetMaxHealth()
    {
        return health + (stamina * 10f);
    }
    public void GainExperience(float amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }
    public void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel += 1000f * level;

        strength += 1f;
        stamina += 1f;
        intelligence += 1f;
        wisdom += 1f;

        availableStatPoints++;
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
                case "Stamina":
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
        }
        else
        {
            Debug.Log("No stat points available.");
        }
    }
}