using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 0.5f;
    public KeyCode interactKey = KeyCode.R;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            InteractWithNearbyStone();
        }
    }

    void InteractWithNearbyStone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            StatStone stone = hitCollider.GetComponent<StatStone>();
            if (stone != null)
            {
                if (playerStats.availableStatPoints > 0)
                {
                    Debug.Log("Available stat points: " + playerStats.availableStatPoints);
                    SpendStatPointAndApplyBonus(stone.statType);
                    return;
                }
            }
        }
    }

    void SpendStatPointAndApplyBonus(StatStone.StatType statType)
    {
        
        playerStats.SpendStatPoint(statType.ToString()); 
        
        ApplyStatBonus(statType);
    }

    void ApplyStatBonus(StatStone.StatType statType)
    {
        switch (statType)
        {
            case StatStone.StatType.Strength:
                playerStats.strength += 1f;
                break;
            case StatStone.StatType.Agility:
                playerStats.agility += 1f;
                break;
            case StatStone.StatType.Intelligence:
                playerStats.intelligence += 1f;
                break;
            case StatStone.StatType.Wisdom:
                playerStats.wisdom += 1f;
                break;
        }
        Debug.Log($"Получено +1 к {statType}. Потрачено 1 очко.");
    }
}
