using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public SlotSelectController slotSelect;
    public void OnNewGameClicked()
    {
        SaveLoadManager.DeleteAll();
        PlayerSession.SelectedSlot=1;
        SceneManager.LoadScene("GameScene");
    }
    public void OnContinueClicked() { slotSelect.ShowContinue(); }
    public void OnSettingsClicked(){  }
    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
}