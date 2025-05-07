using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int    level;
    public float  currentExp, expToNextLevel;
    public float  strength, stamina, intelligence, wisdom;
    public Vector3 playerPosition;
    public float  playerCurrentHealth;
}

public static class SaveLoadManager
{
    static string PathFor(int slot) => Path.Combine(Application.persistentDataPath, $"save_slot{slot}.json");

    public static void SaveGame(int slot, PlayerStats stats, HealthPlayerController hp)
    {
        var data = new SaveData {
            level = stats.GetLevel(),
            currentExp = stats.GetCurrentExp(),
            expToNextLevel = stats.GetExpToNextLevel(),
            strength = stats.strength,
            stamina = stats.stamina,
            intelligence = stats.intelligence,
            wisdom = stats.wisdom,
            playerPosition = stats.transform.position,
            playerCurrentHealth = hp.GetCurrentHealth()
        };
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(data, true));
    }

    public static bool LoadGame(int slot, PlayerStats stats, HealthPlayerController hp)
    {
        var p = PathFor(slot);
        if (!File.Exists(p)) return false;
        var d = JsonUtility.FromJson<SaveData>(File.ReadAllText(p));
        stats.SetLevel(d.level, d.currentExp, d.expToNextLevel);
        stats.strength = d.strength; stats.stamina = d.stamina;
        stats.intelligence = d.intelligence; stats.wisdom = d.wisdom;
        stats.transform.position = d.playerPosition;
        hp.SetHealth(d.playerCurrentHealth);
        return true;
    }

    public static bool HasSave(int slot) => File.Exists(PathFor(slot));

    public static void DeleteSlot(int slot)
    {
        var p = PathFor(slot); 
        if (File.Exists(p)) File.Delete(p);
    }

    public static void DeleteAll()
    {
        for (int i=1;i<=3;i++) DeleteSlot(i);
    }
}