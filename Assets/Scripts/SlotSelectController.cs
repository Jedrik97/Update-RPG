using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Zenject;

public class SlotSelectController : MonoBehaviour
{
    [Header("Panels & Buttons (same Canvas)")]
    public GameObject panel;           // SaveSlotSelectPanel
    public GameObject buttonContainer; // Panel с основными кнопками (Pause) или пустым контейнером (MainMenu)

    public Button[] slotButtons;       
    public Button cancelButton;        

    private PlayerStats _stats;
    private HealthPlayerController _hp;
    private PlayerInventory _inv;
    private PauseMenuController _pauseMenu;  // может быть null на главном меню

    enum Mode { Continue, Save, Load }
    Mode mode;

    [Inject]
    public void Construct(
        PlayerStats stats,
        HealthPlayerController hp,
        PlayerInventory inv,
        // Опциональная инъекция — если PauseMenuController не найден, Zenject впихнёт null
        [InjectOptional] PauseMenuController pauseMenu)
    {
        _stats     = stats;
        _hp        = hp;
        _inv       = inv;
        _pauseMenu = pauseMenu;
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

    void PopulateSlotButtons()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "save_slot*.json");
        var last3 = files.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).Take(3).ToArray();

        for (int i = 0; i < slotButtons.Length; i++)
        {
            var btn   = slotButtons[i];
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            btn.onClick.RemoveAllListeners();

            if (i < last3.Length)
            {
                int slot = ExtractSlotNumber(last3[i]);
                label.text = File.GetLastWriteTime(last3[i]).ToString("dd.MM.yyyy HH:mm");
                int captured = slot;
                btn.onClick.AddListener(() => OnSlotButton(captured));
                btn.gameObject.SetActive(true);
            }
            else if (mode == Mode.Save)
            {
                int newSlot = GetNextAvailableSlotNumber(files);
                label.text  = "Пустой слот";
                btn.onClick.AddListener(() => OnSlotButton(newSlot));
                btn.gameObject.SetActive(true);
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    int ExtractSlotNumber(string path)
        => int.Parse(Path.GetFileNameWithoutExtension(path).Split("save_slot").Last());

    int GetNextAvailableSlotNumber(string[] files)
    {
        var used = files.Select(f => ExtractSlotNumber(f)).ToList();
        for (int s = 1; s <= 3; s++)
            if (!used.Contains(s))
                return s;
        var oldest = files.OrderBy(f => File.GetLastWriteTimeUtc(f)).First();
        return ExtractSlotNumber(oldest);
    }

    void OnSlotButton(int slot)
    {
        switch (mode)
        {
            case Mode.Continue:
                if (SaveLoadManager.HasSave(slot))
                {
                    PlayerSession.SelectedSlot = slot;
                    SceneManager.LoadScene("GameScene");
                }
                break;

            case Mode.Save:
                SaveLoadManager.SaveGame(slot, _stats, _hp, _inv);
                HidePanel();
                if (buttonContainer!=null) buttonContainer.SetActive(true);
                break;

            case Mode.Load:
                if (SaveLoadManager.LoadGame(slot, _stats, _hp, _inv))
                {
                    HidePanel();
                    // Если мы в паузе — закрыть её
                    _pauseMenu?.ClosePauseMenu();
                }
                break;
        }
    }
}
