using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action OnJumpInput;
    public static event Action OnPauseInput;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        OnMoveInput?.Invoke(new Vector2(h, v).normalized);

        if (Input.GetKeyDown(KeyCode.Space))
            OnJumpInput?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnPauseInput?.Invoke();  
    }
}