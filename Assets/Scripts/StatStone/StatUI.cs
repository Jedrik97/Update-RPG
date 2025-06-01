using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{
    public GameObject statPanel;

    public TMP_Text strengthText;
    public TMP_Text staminaText;
    public TMP_Text intelligenceText;
    public TMP_Text wisdomText;

    public TMP_Text statPointsText; // Новое поле

    public void Show(PlayerStats stats, string statToHighlight)
    {
        strengthText.text = Format("Strength", stats.strength, statToHighlight);
        staminaText.text = Format("Stamina", stats.stamina, statToHighlight);
        intelligenceText.text = Format("Intelligence", stats.intelligence, statToHighlight);
        wisdomText.text = Format("Wisdom", stats.wisdom, statToHighlight);

        statPointsText.text = $"Available Stat Points: {stats.availableStatPoints}";

        statPanel.SetActive(true);
    }

    public void Hide()
    {
        statPanel.SetActive(false);
    }

    private string Format(string name, float value, string highlight)
    {
        return name == highlight ? $"{name}: {value} +1" : $"{name}: {value}";
    }
}