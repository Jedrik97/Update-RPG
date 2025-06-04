using UnityEngine;
using UnityEngine.UI;

public class KeyPressButtonUI : MonoBehaviour
{
    [Header("Ссылки на UI Buttons 1–5")]
    [SerializeField] private Button key1Button;
    [SerializeField] private Button key2Button;
    [SerializeField] private Button key3Button;
    [SerializeField] private Button key4Button;
    [SerializeField] private Button key5Button;
    
    private Color key1NormalColor;
    private Color key2NormalColor;
    private Color key3NormalColor;
    private Color key4NormalColor;
    private Color key5NormalColor;
    
    private Color key1PressedColor;
    private Color key2PressedColor;
    private Color key3PressedColor;
    private Color key4PressedColor;
    private Color key5PressedColor;
    
    private Vector3 normalScale = Vector3.one;
    private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);

    private void Start()
    {

        var colors1 = key1Button.colors;
        key1NormalColor = colors1.normalColor;
        key1PressedColor = colors1.pressedColor;
        
        var colors2 = key2Button.colors;
        key2NormalColor = colors2.normalColor;
        key2PressedColor = colors2.pressedColor;
       
        var colors3 = key3Button.colors;
        key3NormalColor = colors3.normalColor;
        key3PressedColor = colors3.pressedColor;
        
        var colors4 = key4Button.colors;
        key4NormalColor = colors4.normalColor;
        key4PressedColor = colors4.pressedColor;
       
        var colors5 = key5Button.colors;
        key5NormalColor = colors5.normalColor;
        key5PressedColor = colors5.pressedColor;
    }

    private void Update()
    {

        HandleButtonKey(KeyCode.Alpha1, key1Button, ref key1NormalColor, ref key1PressedColor);
        HandleButtonKey(KeyCode.Alpha2, key2Button, ref key2NormalColor, ref key2PressedColor);
        HandleButtonKey(KeyCode.Alpha3, key3Button, ref key3NormalColor, ref key3PressedColor);
        HandleButtonKey(KeyCode.Alpha4, key4Button, ref key4NormalColor, ref key4PressedColor);
        HandleButtonKey(KeyCode.Alpha5, key5Button, ref key5NormalColor, ref key5PressedColor);
    }

    private void HandleButtonKey(KeyCode key, Button button, ref Color normalColor, ref Color pressedColor)
    {
        var img = button.targetGraphic as Image;
        if (img == null) return;

        if (Input.GetKeyDown(key))
        {
            img.color = pressedColor;
            button.transform.localScale = pressedScale;
        }
        else if (Input.GetKeyUp(key))
        {
            img.color = normalColor;
            button.transform.localScale = normalScale;
        }
    }
}
