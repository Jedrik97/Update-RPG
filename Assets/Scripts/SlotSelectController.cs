using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotSelectController : MonoBehaviour
{
    public GameObject panel;
    public PlayerStats stats;
    public HealthPlayerController hp;
    enum Mode{Continue,Save,Load}
    Mode mode;

    public void ShowContinue()
    {
        mode=Mode.Continue; panel.SetActive(true);
    }

    public void ShowSave()
    {
        mode=Mode.Save;     panel.SetActive(true);
    }

    public void ShowLoad()
    {
        mode=Mode.Load;     panel.SetActive(true);
    }
    public void OnSlotButton(int slot)
    {
        switch(mode)
        {
            case Mode.Continue:
                if (SaveLoadManager.HasSave(slot))
                {
                    PlayerSession.SelectedSlot=slot;
                    SceneManager.LoadScene("GameScene");
                }
                break;
            case Mode.Save:
                SaveLoadManager.SaveGame(slot,stats,hp);
                break;
            case Mode.Load:
                SaveLoadManager.LoadGame(slot,stats,hp);
                break;
        }
        panel.SetActive(false);
    }

    public void OnCancel()
    {
        panel.SetActive(false);
    }
}