using UnityEngine;
using TMPro;
using System.Collections;

public class TemporaryMessageUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Панель (Panel) содержащая TextMeshProUGUI")]
    public GameObject panel;
    [Tooltip("Компонент TextMeshProUGUI для вывода сообщений")]
    public TextMeshProUGUI messageText;

    private Coroutine hideCoroutine;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }
    
    public void ShowMessage(string message, float duration = 2f)
    {
        if (panel == null || messageText == null)
        {
            Debug.LogWarning("TemporaryMessageUI: UI элементы не настроены.");
            return;
        }
        
        messageText.text = message;
        panel.SetActive(true);
        
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
        hideCoroutine = null;
    }
}