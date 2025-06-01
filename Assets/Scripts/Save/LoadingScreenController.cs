// LoadingScreenController.cs
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
    private float displayDuration = 2f; // можете настроить

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (loadingPanel)
                loadingPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Обычная загрузка новой сцены с прогресс-баром.
    /// </summary>
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
        progressSlider.value = 0;
    }

    /// <summary>
    /// Показывает загрузочный экран на displayDuration секунд,
    /// обновляет слайдер (учитывая паузу), затем вызывает onComplete.
    /// </summary>
    public void ShowLoadingProcess(System.Action onComplete)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        StartCoroutine(LoadingProcessRoutine(onComplete));
    }

    private IEnumerator LoadingProcessRoutine(System.Action onComplete)
    {
        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.unscaledDeltaTime; // работает, даже если Time.timeScale = 0
            if (progressSlider != null)
                progressSlider.value = Mathf.Clamp01(timer / displayDuration);
            yield return null;
        }

        onComplete?.Invoke();

        // короткая пауза, чтобы увидеть полностью заполненный бар
        yield return new WaitForSecondsRealtime(0.1f);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
        progressSlider.value = 0;
    }
}
