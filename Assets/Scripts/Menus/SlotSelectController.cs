using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class SlotSelectController : MonoBehaviour
{
    [Header("Panels & Buttons (same Canvas)")]
    public GameObject panel;
    public GameObject buttonContainer;

    [Tooltip("Ожидается 3 кнопки (Slot 1, Slot 2, Slot 3)")]
    public Button[] slotButtons;
    public Button cancelButton;

    [Header("Player Respawn Settings")]
    [Tooltip("Тот же префаб игрока, который используется в GameSceneInstaller")]
    public GameObject playerPrefab;
    [Tooltip("Точка спавна, аналогичная той, что в GameSceneInstaller")]
    public Transform spawnPoint;

    private PlayerStats _stats;
    private HealthPlayerController _hp;
    private PlayerInventory _inv;
    private PauseMenuController _pauseMenu;
    private GameManager _gameManager;

    enum Mode { Continue, Save, Load }
    private Mode mode;

    [Inject]
    public void Construct(
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv,
        [InjectOptional] PauseMenuController pauseMenu,
        GameManager gameManager)
    {
        _stats       = stats;
        _hp          = hp;
        _inv         = inv;
        _pauseMenu   = pauseMenu;
        _gameManager = gameManager;
    }

    public void ShowContinue()
    {
        mode = Mode.Continue;
        panel.SetActive(true);
        PopulateSlotButtons();
    }

    public void ShowSave()
    {
        mode = Mode.Save;
        panel.SetActive(true);
        PopulateSlotButtons();
    }

    public void ShowLoad()
    {
        mode = Mode.Load;
        panel.SetActive(true);
        PopulateSlotButtons();
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void OnCancelClicked()
    {
        HidePanel();
        if (buttonContainer != null)
            buttonContainer.SetActive(true);
    }

    public void OnSlot1Clicked() => OnSlotButton(1);
    public void OnSlot2Clicked() => OnSlotButton(2);
    public void OnSlot3Clicked() => OnSlotButton(3);

    private void PopulateSlotButtons()
    {
        for (int i = 1; i <= 3; i++)
        {
            int index = i - 1; // для slotButtons[0..2]
            var btn   = slotButtons[index];
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            btn.onClick.RemoveAllListeners();

            string path = Path.Combine(Application.persistentDataPath, $"save_slot{i}.json");
            bool exists = File.Exists(path);

            if (exists)
            {
                // Отображаем дату последнего сохранения
                var dt = File.GetLastWriteTime(path);
                label.text = dt.ToString("dd.MM.yyyy HH:mm");
                btn.onClick.AddListener(() => OnSlotButton(i));
                btn.gameObject.SetActive(true);
            }
            else if (mode == Mode.Save)
            {
                // В режиме «Сохранить» пустой слот тоже доступен
                label.text = "Пустой слот";
                btn.onClick.AddListener(() => OnSlotButton(i));
                btn.gameObject.SetActive(true);
            }
            else
            {
                // В режимах Continue/Load скрываем несуществующие слоты
                btn.gameObject.SetActive(false);
            }
        }

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    private void OnSlotButton(int slot)
    {
        switch (mode)
        {
            case Mode.Continue:
                if (SaveLoadManager.HasSave(slot))
                {
                    PlayerSession.SelectedSlot = slot;
                    HidePanel();
                    if (buttonContainer != null)
                        buttonContainer.SetActive(false);

                    // Переходим к сцене игры. SaveLoadInitializer в новой сцене применит данные.
                    LoadingScreenController.Instance.LoadScene("GameScene");
                }
                break;

            case Mode.Save:
                // Сохраняем текущие данные; передаём GameManager для флага bossDefeated
                SaveLoadManager.SaveGame(slot, _stats, _hp, _inv, _gameManager);
                HidePanel();
                if (buttonContainer != null)
                    buttonContainer.SetActive(true);
                break;

            case Mode.Load:
                if (SaveLoadManager.HasSave(slot))
                {
                    // При загрузке из паузы необходимо удалить старого игрока,
                    // затем создать нового из префаба и применить к нему данные.
                    LoadingScreenController.Instance.ShowLoadingProcess(() =>
                    {
                        // 1) Удаляем старого игрока
                        if (_stats != null)
                        {
                            Destroy(_stats.gameObject);
                        }

                        // 2) Инстанцируем нового игрока из префаба в spawnPoint
                        var playerGO = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                        playerGO.SetActive(true);

                        // 3) Получаем нужные компоненты у нового экземпляра
                        var newStats = playerGO.GetComponent<PlayerStats>();
                        var newHp    = playerGO.GetComponent<HealthPlayerController>();
                        var newInv   = playerGO.GetComponent<PlayerInventory>();

                        // 4) Заменяем ссылки в контроллере
                        _stats = newStats;
                        _hp    = newHp;
                        _inv   = newInv;

                        // 5) Загружаем сохранение на нового игрока
                        SaveData data = SaveLoadManager.LoadGame(slot, _stats, _hp, _inv);
                        if (data != null && data.bossDefeated && _gameManager != null)
                        {
                            _gameManager.SetBossDefeatedFromSave();
                        }

                        // 6) Закрываем панель и меню паузы
                        HidePanel();
                        _pauseMenu?.ClosePauseMenu();
                    });
                }
                break;
        }
    }
}
