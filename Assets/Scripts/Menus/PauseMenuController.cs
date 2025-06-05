using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels on this Canvas")]
    public GameObject pausePanel;         // Панель самого меню паузы
    public GameObject buttonContainer;    // Контейнер с кнопками «Сохранить», «Загрузить» и т.д.

    [Header("Slot Select")]
    public SlotSelectController slotSelect; // Скрипт, отвечающий за выбор слота загрузки/сохранения

    private PlayerStats _stats;
    private HealthPlayerController _hp;
    private PlayerInventory _inv;
    private GameManager _gameManager;
    private int _nextSaveSlot;

    public static bool IsPaused { get; private set; }

    [Inject]
    public void Construct(
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv,
        GameManager gameManager)
    {
        _stats = stats;
        _hp = hp;
        _inv = inv;
        _gameManager = gameManager;
    }

    void Start()
    {
        // Читаем из PlayerPrefs следующий слот для сохранения (1–3)
        _nextSaveSlot = PlayerPrefs.GetInt("NextSaveSlot", 1);
        if (_nextSaveSlot < 1 || _nextSaveSlot > 3)
            _nextSaveSlot = 1;
    }

    void OnEnable()
    {
        // Подписываемся на событие паузы (Esc или другая кнопка, которая вызывает PlayerInput.OnPauseInput)
        PlayerInput.OnPauseInput += TogglePause;
    }

    void OnDisable()
    {
        PlayerInput.OnPauseInput -= TogglePause;
    }

    void TogglePause()
    {
        // Если меню смерти уже показано, не даём открыть меню паузы
        if (_gameManager != null && _gameManager.IsDeathMenuVisible)
            return;

        bool shouldPause = !pausePanel.activeSelf;

        // Включаем/выключаем саму панель и контейнер с кнопками
        pausePanel.SetActive(shouldPause);
        buttonContainer.SetActive(shouldPause);
        // При открытии паузы скрываем панель выбора слота (если она открыта)
        slotSelect.HidePanel();

        // Останавливаем/возобновляем время
        Time.timeScale = shouldPause ? 0f : 1f;
        IsPaused = shouldPause;

        if (shouldPause)
            CursorManager.Instance?.ShowCursor();
        else
            CursorManager.Instance?.HideCursor();
    }

    public void OnSaveClicked()
    {
        // Сохраняем текущую игру в слот _nextSaveSlot
        SaveLoadManager.SaveGame(_nextSaveSlot, _stats, _hp, _inv, _gameManager);

        // Инкрементируем слот (1→2→3→1)
        _nextSaveSlot = _nextSaveSlot % 3 + 1;
        PlayerPrefs.SetInt("NextSaveSlot", _nextSaveSlot);
        PlayerPrefs.Save();
    }

    public void OnLoadClicked()
    {
        // Скрываем контейнер кнопок и отображаем выбор слота загрузки
        buttonContainer.SetActive(false);
        slotSelect.ShowLoad();
    }

    public void OnExitToMainMenuClicked()
    {
        // При выходе в главное меню сбрасываем Time.timeScale и статус IsPaused
        Time.timeScale = 1f;
        IsPaused = false;
        CursorManager.Instance?.ShowCursor();
        SceneManager.LoadScene("MainMenu");
    }

    public void ClosePauseMenu()
    {
        // Закрываем меню паузы, возвращаем время в 1×
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        CursorManager.Instance?.HideCursor();
    }
}
