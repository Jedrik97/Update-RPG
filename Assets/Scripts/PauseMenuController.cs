
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
        
        if (!pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
            buttonContainer.SetActive(true);
            slotSelect.HidePanel();
            Time.timeScale = 0f;
        }
        else
        {
            
            pausePanel.SetActive(false);
            slotSelect.HidePanel();
            Time.timeScale = 1f;
        }
    }

    
    public void OnSaveClicked()
    {
        SaveLoadManager.SaveGame(_nextSaveSlot, _stats, _hp, _inv);
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    
    
    
    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
