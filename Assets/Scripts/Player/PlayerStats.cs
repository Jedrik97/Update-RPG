using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 100f;
    private float expMultiplier = 1.2f;
    
    public float strength = 10f;
    public float agility = 10f;
    public float intelligence = 10f;

    public void EnemyKilled()
    {
        float expGain = 50f; 
        currentExp += expGain;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel *= expMultiplier;

        strength += 2f;
        agility += 2f;
        intelligence += 2f;
    }
}