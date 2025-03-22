using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action OnJumpInput;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontal, vertical).normalized;

        OnMoveInput?.Invoke(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput?.Invoke();
        }
    }
}