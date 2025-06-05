using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels on this Canvas")]
    public GameObject pausePanel;
    public GameObject buttonContainer;

    [Header("Slot Select")]
    public SlotSelectController slotSelect;

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
        _nextSaveSlot = PlayerPrefs.GetInt("NextSaveSlot", 1);
        if (_nextSaveSlot < 1 || _nextSaveSlot > 3)
            _nextSaveSlot = 1;
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
        if (_gameManager != null && _gameManager.IsDeathMenuVisible)
            return;

        bool shouldPause = !pausePanel.activeSelf;

        if (shouldPause)
        {
            // Входим в паузу
            pausePanel.SetActive(true);
            buttonContainer.SetActive(true);
            slotSelect.HidePanel();

            Time.timeScale = 0f;
            IsPaused = true;

            CursorManager.Instance?.ShowCursor();
        }
        else
        {
            // Выходим из паузы

            // 1) сразу скрываем курсор
            CursorManager.Instance?.HideCursor();

            // 2) деактивируем все элементы паузы
            pausePanel.SetActive(false);
            buttonContainer.SetActive(false);
            slotSelect.HidePanel();

            // 3) возвращаем время
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }


    public void OnSaveClicked()
    {
        SaveLoadManager.SaveGame(_nextSaveSlot, _stats, _hp, _inv, _gameManager);

        _nextSaveSlot = _nextSaveSlot % 3 + 1;
        PlayerPrefs.SetInt("NextSaveSlot", _nextSaveSlot);
        PlayerPrefs.Save();
    }

    public void OnLoadClicked()
    {
        buttonContainer.SetActive(false);
        slotSelect.ShowLoad();
    }

    public void OnExitToMainMenuClicked()
    {
        // При выходе на главное меню тоже сразу возвращаем время и курсор
        Time.timeScale = 1f;
        IsPaused = false;
        CursorManager.Instance?.ShowCursor();
        SceneManager.LoadScene("MainMenu");
    }

    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);

        // Прячем курсор
        CursorManager.Instance?.HideCursor();

        Time.timeScale = 1f;
        IsPaused = false;
    }
}
