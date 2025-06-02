using UnityEngine;
using TMPro;

public class PlayerStatsMenu : MonoBehaviour
{
    public GameObject statsPanel;
    public TMP_Text strengthText;
    public TMP_Text staminaText;
    public TMP_Text intelligenceText;
    public TMP_Text wisdomText;
    public TMP_Text statPointsText;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
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
        bool isActive = statsPanel.activeSelf;
        statsPanel.SetActive(!isActive);

        if (!isActive)
            UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (!playerStats) return;

        strengthText.text = $"Strength: {playerStats.strength}";
        staminaText.text = $"Stamina: {playerStats.stamina}";
        intelligenceText.text = $"Intelligence: {playerStats.intelligence}";
        wisdomText.text = $"Wisdom: {playerStats.wisdom}";

        statPointsText.text = $"Available Stat Points: {playerStats.availableStatPoints}";
    }
}
