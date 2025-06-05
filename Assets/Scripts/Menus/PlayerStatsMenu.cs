using UnityEngine;
using TMPro;

public class PlayerStatsMenu : MonoBehaviour
{
    [Header("UI References (assign in Inspector)")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text wisdomText;
    [SerializeField] private TMP_Text statPointsText;

    [Header("PlayerStats Reference (assign in Inspector)")]
    [SerializeField] private PlayerStats playerStats;

    private void Start()
    {
        if (statsPanel == null)
        {
            Debug.LogError("[PlayerStatsMenu] statsPanel is not assigned!");
            return;
        }
        
        statsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (statsPanel == null) return;

        bool isActive = statsPanel.activeSelf;
        statsPanel.SetActive(!isActive);

        if (!isActive)
        {
            if (CursorManager.Instance)
                CursorManager.Instance.ShowCursor();
            else
                Debug.LogWarning("[PlayerStatsMenu] No CursorManager found in scene!");

            UpdateStatsUI();
        }
        else
        {
            if (CursorManager.Instance)
                CursorManager.Instance.HideCursor();

            TooltipUI.Instance?.HideTooltip();
        }
    }

    private void UpdateStatsUI()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerStatsMenu] playerStats reference is null, UI will not update.");
            return;
        }

        strengthText.text     = $"Strength: {playerStats.strength}";
        staminaText.text      = $"Stamina: {playerStats.stamina}";
        intelligenceText.text = $"Intelligence: {playerStats.intelligence}";
        wisdomText.text       = $"Wisdom: {playerStats.wisdom}";
        statPointsText.text   = $"Available Stat Points: {playerStats.availableStatPoints}";
    }
}
