using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public int Gold { get; private set; } = 0;
    public int HealthPotions { get; private set; } = 0;
    private const int MaxPotions = 5;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnPotionsChanged;

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
        return false;
    }

    public bool BuyHealthPotion(int cost = 2)
    {
        if (HealthPotions >= MaxPotions)
        {
            return false;
        }

        if (SpendGold(cost))
        {
            HealthPotions++;
            OnPotionsChanged?.Invoke(HealthPotions);
            return true;
        }
        return false;
    }

    public bool UseHealthPotion(HealthPlayerController healthController, float healAmount = 50f)
    {
        if (HealthPotions > 0)
        {
            HealthPotions--;
            OnPotionsChanged?.Invoke(HealthPotions);
            healthController.Heal(healAmount);
            return true;
        }
        return false;
    }
}