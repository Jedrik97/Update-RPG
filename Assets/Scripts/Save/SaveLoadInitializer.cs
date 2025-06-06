using Zenject;

public class SaveLoadInitializer : IInitializable
{
    readonly PlayerStats _stats;
    readonly HealthPlayerController _hp;
    readonly PlayerInventory _inv;
    readonly GameManager _gameManager;

    public SaveLoadInitializer(
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv,
        GameManager gameManager)
    {
        _stats       = stats;
        _hp          = hp;
        _inv         = inv;
        _gameManager = gameManager;
    }

    public void Initialize()
    {
        int slot = PlayerSession.SelectedSlot;

        SaveData data = SaveLoadManager.LoadGame(slot, _stats, _hp, _inv);
        if (data != null)
        {
            if (data.bossDefeated)
            {
                _gameManager.SetBossDefeatedFromSave();
            }
            else
            {
                _gameManager.ShowBossObjectivePanelIfNeeded();
            }
        }
    }
}