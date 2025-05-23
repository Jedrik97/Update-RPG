using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenuController : MonoBehaviour
{
    public SlotSelectController slotSelect;

    public void OnNewGameClicked()
    {
        SaveLoadManager.DeleteAll();
        PlayerSession.SelectedSlot = 1;
        SceneManager.LoadScene("GameScene");
    }
    
    public void OnContinueClicked()
    {
        slotSelect.buttonContainer = null; 
        slotSelect.ShowContinue();
    }

    public void OnSettingsClicked() { /*…*/ }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
}