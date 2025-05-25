using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance { get; private set; }

    [SerializeField]
    [Tooltip("Ссылка на панель загрузки (GameObject), который нужно показывать/скрывать")]
    private GameObject loadingPanel;

    [SerializeField]
    [Tooltip("Ссылка на слайдер прогресса (0–1)")]
    private Slider progressSlider;

    [SerializeField]
    [Tooltip("Длительность показа экрана загрузки в секундах")]
    private float displayDuration = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (loadingPanel)
                loadingPanel.SetActive(false);
        }
    }
    
    public void LoadScene(string sceneName)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.deltaTime;
            if (progressSlider != null)
                progressSlider.value = Mathf.Clamp01(timer / displayDuration);
            yield return null;
        }
        

        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
}