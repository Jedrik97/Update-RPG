using Zenject;

public class SaveLoadInitializer : IInitializable
{
    readonly PlayerStats _stats;
    readonly HealthPlayerController _hp;
    readonly PlayerInventory _inv;

    public SaveLoadInitializer(
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv)
    {
        _stats = stats;
        _hp    = hp;
        _inv   = inv;
    }

    public void Initialize()
    {
        int slot = PlayerSession.SelectedSlot;
        if (SaveLoadManager.HasSave(slot))
        {
            SaveLoadManager.LoadGame(slot, _stats, _hp, _inv);
        }
    }
}