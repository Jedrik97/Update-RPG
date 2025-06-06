using System.IO;
using UnityEngine;

public static class SaveLoadManager
{   
    static string PathFor(int slot) => Path.Combine(Application.persistentDataPath, $"save_slot{slot}.json");
    public static void SaveGame(int slot,
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv,
        GameManager gameManager)
    {
        var data = new SaveData
        {
            level               = stats.GetLevel(),
            currentExp          = stats.GetCurrentExp(),
            expToNextLevel      = stats.GetExpToNextLevel(),
            strength            = stats.strength,
            stamina             = stats.stamina,
            intelligence        = stats.intelligence,
            wisdom              = stats.wisdom,
            availableStatPoints = stats.availableStatPoints,
            gold                = inv.Gold,
            healthPotions       = inv.HealthPotions,
            playerPosition      = stats.transform.position,
            playerCurrentHealth = hp.GetCurrentHealth(),
            
            bossDefeated        = (gameManager != null && gameManager.BossDefeated)
        };

        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(data, true));
    }
    
    public static SaveData LoadGame(int slot,
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv)
    {
        var path = PathFor(slot);
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        var d = JsonUtility.FromJson<SaveData>(json);
        
        stats.SetLevel(d.level, d.currentExp, d.expToNextLevel);
        stats.strength            = d.strength;
        stats.stamina             = d.stamina;
        stats.intelligence        = d.intelligence;
        stats.wisdom              = d.wisdom;
        stats.availableStatPoints = d.availableStatPoints;
        stats.UpdateExpBar();

        inv.SetGold(d.gold);
        inv.SetHealthPotions(d.healthPotions);
        
        var cc = stats.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        stats.transform.position = d.playerPosition;
        if (cc != null) cc.enabled = true;

        hp.SetHealth(d.playerCurrentHealth);

        return d;
    }

    public static bool HasSave(int slot) => File.Exists(PathFor(slot));

    public static void DeleteSlot(int slot)
    {
        var p = PathFor(slot);
        if (File.Exists(p)) File.Delete(p);
    }

    public static void DeleteAll()
    {
        for (int i = 1; i <= 3; i++)
            DeleteSlot(i);
    }
}