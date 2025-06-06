using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance { get; private set; }

    [SerializeField]
    private GameObject loadingPanel;

    [SerializeField]
    private Slider progressSlider;

    [SerializeField]
    private float displayDuration = 2f;

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
    public void LoadScene(string sceneName)
    {
        if (loadingPanel)
            loadingPanel.SetActive(true);
        CursorManager.Instance?.HideCursor();
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
            if (progressSlider)
                progressSlider.value = Mathf.Clamp01(timer / displayDuration);
            yield return null;
        }

        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

        if (loadingPanel)
            loadingPanel.SetActive(false);
        progressSlider.value = 0;
        
    }
    
    public void ShowLoadingProcess(System.Action onComplete)
    {
        if (loadingPanel)
            loadingPanel.SetActive(true);
        CursorManager.Instance?.HideCursor();
        StartCoroutine(LoadingProcessRoutine(onComplete));
        
    }

    private IEnumerator LoadingProcessRoutine(System.Action onComplete)
    {
        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (progressSlider)
                progressSlider.value = Mathf.Clamp01(timer / displayDuration);
            yield return null;
        }

        onComplete?.Invoke();
        yield return new WaitForSecondsRealtime(0.1f);

        if (loadingPanel)
            loadingPanel.SetActive(false);
        progressSlider.value = 0;
    }
}