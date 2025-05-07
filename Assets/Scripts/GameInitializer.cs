using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private HealthPlayerController hp;
    [SerializeField] private Transform spawnPoint; 

    void Start()
    {
        int slot = PlayerSession.SelectedSlot;
        bool loaded = slot >= 1
                      && slot <= 3
                      && SaveLoadManager.LoadGame(slot, stats, hp);

        if (!loaded)
        {
            if (spawnPoint != null)
                stats.transform.position = spawnPoint.position;
            hp.SetHealth(hp.maxHealth);
            stats.UpdateExpBar();
        }
    }
}