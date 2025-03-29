using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 1000f;

    private float baseExp = 1000f;

    public float strength = 10f;
    public float agility = 10f;
    public float intelligence = 10f;
    public float wisdom = 10f;

    // Новая переменная для отслеживания доступных очков характеристик
    public int availableStatPoints = 0;

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

        // При уровне увеличиваем характеристики на 1 каждую
        strength += 1f;
        agility += 1f;
        intelligence += 1f;
        wisdom += 1f;

        // Даем дополнительное очко для распределения
        availableStatPoints++; // Один дополнительный очко на выбор при каждом уровне
    }

    // Метод для распределения очков характеристик
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
                    agility += 1f;
                    break;
                case "Intelligence":
                    intelligence += 1f;
                    break;
                case "Wisdom":
                    wisdom += 1f;
                    break;
            }

            availableStatPoints--; // Очко потрачено
            Debug.Log($"Потрачено 1 очко на {stat}. Осталось {availableStatPoints} очков.");
        }
        else
        {
            Debug.Log("Нет доступных очков для распределения.");
        }
    }
}