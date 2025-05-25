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

        // Сразу отрисовываем текущее состояние
        UpdateGold(_inventory.Gold);
        UpdatePotions(_inventory.HealthPotions);

        // Подписываемся на события
        _inventory.OnGoldChanged += UpdateGold;
        _inventory.OnPotionsChanged += UpdatePotions;
    }

    void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnGoldChanged -= UpdateGold;
            _inventory.OnPotionsChanged -= UpdatePotions;
        }
    }

    private void UpdateGold(int value)
    {
        if (goldText == null)
        {
            Debug.LogWarning("[InventoryUI] Поле goldText не назначено в Инспекторе!");
            return;
        }
        goldText.text = $"Gold: {value}";
    }

    private void UpdatePotions(int value)
    {
        if (potionText == null)
        {
            Debug.LogWarning("[InventoryUI] Поле potionText не назначено в Инспекторе!");
            return;
        }
        potionText.text = $"Potions: {value}/{MaxPotions}";
    }
}