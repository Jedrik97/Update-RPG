using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove;
    public static event Action<bool> OnJump;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 100f;
    public float jumpForce = 2f;
    public float gravity = 9.81f;
    public float rotationSmoothTime = 0.1f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private Transform cameraTransform;
    private bool isRunning = false;
    private float rotationVelocity;
    private bool canMove = true;

    private void OnEnable()
    {
        PlayerInput.OnMoveInput += HandleMoveInput;
        PlayerInput.OnJumpInput += HandleJumpInput;
        PlayerCombat.OnAttackStateChanged += HandleAttackStateChanged;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void HandleMoveInput(Vector2 input)
    {
        if (!canMove) return;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (input.magnitude < 0.1f)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            OnMove?.Invoke(input.x, input.y, isRunning);
            return;
        }

        
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; 
        right.y = 0;   
        forward.Normalize();
        right.Normalize();
        
        Vector3 movementDirection = (forward * input.y + right * input.x).normalized;
        moveDirection = new Vector3(movementDirection.x * currentSpeed, moveDirection.y, movementDirection.z * currentSpeed);
        
        if (movementDirection.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
        }

        OnMove?.Invoke(input.x, input.y, isRunning);
    }

    private void HandleJumpInput()
    {
        if (characterController.isGrounded && !isJumping && canMove) 
        {
            isJumping = true;
            moveDirection.y = jumpForce;
            OnJump?.Invoke(true);
        }
    }

    void Update()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else if (isJumping)
        {
            isJumping = false;
            OnJump?.Invoke(false);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleAttackStateChanged(bool isAttacking)
    {
        canMove = !isAttacking;
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveInput -= HandleMoveInput;
        PlayerInput.OnJumpInput -= HandleJumpInput;
        PlayerCombat.OnAttackStateChanged -= HandleAttackStateChanged;
    }
}
