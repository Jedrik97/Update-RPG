using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DeathMenuController : MonoBehaviour
{
    [Header("Death UI Panel")]
    public GameObject deathPanel;   // Панель, которую показываем при смерти

    public SlotSelectController slotSelect;
    public GameObject buttonContainer;

    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }
    
    public void ShowDeathMenu()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void OnLoadClicked()
    {
        Time.timeScale = 1f;

        if (slotSelect)
        {
            buttonContainer.SetActive(false);
            slotSelect.ShowLoad();
        }
        else
        {
            Debug.LogWarning("DeathMenuController: SlotSelectController не назначен!");
        }
    }

    public void OnExitToMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}