using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Zenject;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Ссылка на контроллер выбора слотов")]
    public SlotSelectController slotSelect;

    public void OnNewGameClicked()
    {
        SaveLoadManager.DeleteAll();
        PlayerSession.SelectedSlot = 1;
        LoadingScreenController.Instance.LoadScene("GameScene");
    }
    
    public void OnContinueClicked()
    {
        slotSelect.buttonContainer = null;
        slotSelect.ShowContinue();
    }

    public void OnSettingsClicked()
    {
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}