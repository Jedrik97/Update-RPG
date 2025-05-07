using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;
    public SlotSelectController slotSelect;
    bool isPaused;

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
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnSaveClicked() => slotSelect.ShowSave();
    public void OnLoadClicked() => slotSelect.ShowLoad();

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}