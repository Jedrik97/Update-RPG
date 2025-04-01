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

    public float health = 10f;

    // Простой метод для получения максимального здоровья
    public float GetMaxHealth()
    {
        return health + (stamina * 10f);
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
}