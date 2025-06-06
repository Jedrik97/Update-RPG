using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    public SlotSelectController slotSelect;
    
    private void Start()
    {
        CursorManager.Instance.ShowCursor();
    }

    public void OnNewGameClicked()
    {
        SaveLoadManager.DeleteAll();
        PlayerSession.SelectedSlot = 1;
        LoadingScreenController.Instance.LoadScene("GameScene");
    }
    
    public void OnContinueClicked()
    {
        slotSelect.buttonContainer.SetActive(false);
        slotSelect.ShowContinue();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}