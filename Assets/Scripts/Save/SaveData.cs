using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int    level;
    public float  currentExp, expToNextLevel;
    public float  strength, stamina, intelligence, wisdom;
    public int    availableStatPoints;
    public int    gold;
    public int    healthPotions;
    public Vector3 playerPosition;
    public float  playerCurrentHealth;
    
    public bool bossDefeated;
}