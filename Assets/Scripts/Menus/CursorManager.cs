using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Custom Cursor Texture")]
    public Texture2D customCursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyCustomCursor();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ApplyCustomCursor()
    {
        if (customCursorTexture != null)
        {
            Cursor.SetCursor(customCursorTexture, hotspot, cursorMode);
        }
    }
}