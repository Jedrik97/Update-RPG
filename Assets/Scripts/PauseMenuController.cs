// PauseMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels on this Canvas")]
    public GameObject pausePanel;       // корневая панель паузы (фон + всё внутри)
    public GameObject buttonContainer;  // внутренняя панель с Save / Load / Exit

    [Header("Slot Select")]
    public SlotSelectController slotSelect;

    private PlayerStats _stats;
    private HealthPlayerController _hp;
    private PlayerInventory _inv;
    private int _nextSaveSlot;

    [Inject]
    public void Construct(PlayerStats stats, HealthPlayerController hp, PlayerInventory inv)
    {
        _stats = stats;
        _hp    = hp;
        _inv   = inv;
    }

    void OnEnable()
    {
        PlayerInput.OnPauseInput += TogglePause;
    }

    void OnDisable()
    {
        PlayerInput.OnPauseInput -= TogglePause;
    }

    void TogglePause()
    {
        // Если пауза закрыта — открыть
        if (!pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
            buttonContainer.SetActive(true);
            slotSelect.HidePanel();
            Time.timeScale = 0f;
        }
        else
        {
            // Если уже открыта — закрыть
            pausePanel.SetActive(false);
            slotSelect.HidePanel();
            Time.timeScale = 1f;
        }
    }

    // Привязать к кнопке Save через On Click()
    public void OnSaveClicked()
    {
        SaveLoadManager.SaveGame(_nextSaveSlot, _stats, _hp, _inv);
        _nextSaveSlot = _nextSaveSlot % 3 + 1;
        PlayerPrefs.SetInt("NextSaveSlot", _nextSaveSlot);
        PlayerPrefs.Save();
    }

    // Привязать к кнопке Load через On Click()
    public void OnLoadClicked()
    {
        buttonContainer.SetActive(false);
        slotSelect.ShowLoad();
    }

    // Привязать к кнопке Exit через On Click()
    public void OnExitToMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Полностью скрываем pausePanel и возвращаем время.
    /// </summary>
    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
