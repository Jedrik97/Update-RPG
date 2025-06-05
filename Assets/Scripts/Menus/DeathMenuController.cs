using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DeathMenuController : MonoBehaviour
{
    [Header("Death UI Panel")]
    public GameObject deathPanel;

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

        // Останавливаем игру, показываем курсор
        Time.timeScale = 0f;
        CursorManager.Instance?.ShowCursor();
    }

    public void OnLoadClicked()
    {
        // Оставляем Time.timeScale = 0, чтобы игра не шла,
        // пока игрок не выбрал слот
        Time.timeScale = 0f;
        
        // Показываем курсор, иначе его не будет видно на экране выбора слота
        CursorManager.Instance?.ShowCursor();

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
        CursorManager.Instance?.ShowCursor();
        SceneManager.LoadScene("MainMenu");
    }
}