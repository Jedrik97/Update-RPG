using UnityEngine;
using TMPro;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI potionText;

    private PlayerInventory _inventory;
    private const int MaxPotions = 5;

    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;
        
        UpdateGold(_inventory.Gold);
        UpdatePotions(_inventory.HealthPotions);
        
        _inventory.OnGoldChanged += UpdateGold;
        _inventory.OnPotionsChanged += UpdatePotions;
    }

    void OnDestroy()
    {
        if (_inventory)
        {
            _inventory.OnGoldChanged -= UpdateGold;
            _inventory.OnPotionsChanged -= UpdatePotions;
        }
    }

    private void UpdateGold(int value)
    {
        if (goldText == null)
        {
            Debug.LogWarning("[InventoryUI]");
            return;
        }
        goldText.text = $"Gold: {value}";
    }

    private void UpdatePotions(int value)
    {
        if (potionText == null)
        {
            Debug.LogWarning("[InventoryUI]");
            return;
        }
        potionText.text = $"Potions: {value}/{MaxPotions}";
    }
}