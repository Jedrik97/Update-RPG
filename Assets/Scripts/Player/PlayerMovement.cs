using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove;
    public static event Action<bool> OnJump;

    [Header("Movement Settings")]
    public float walkSpeed         = 2f;
    public float runSpeed          = 4f;
    public float gravity           = 9.81f;
    public float jumpForce         = 2f;
    
    [Tooltip("Сглаживание поворота при ходьбе")]
    public float rotationSmoothTime = 0.2f;
    
    [Tooltip("Затухание Speed")]
    public float speedDampTime     = 0.15f;
    [Tooltip("Затухание Direction")]
    public float directionDampTime = 0.15f;

    [Header("References (assign in Inspector)")]
    public CharacterController characterController;
    public Animator            animator;

    private Transform cameraTransform;
    private Vector3    moveDirection;
    private float      rotationVelocity;
    private bool       isJumping;
    private bool       isRunning;
    private bool       canMove = true;

    private void OnEnable()
    {
        PlayerInput.OnMoveInput           += HandleMoveInput;
        PlayerInput.OnJumpInput           += HandleJumpInput;
        PlayerCombat.OnAttackStateChanged += HandleAttackStateChanged;
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveInput           -= HandleMoveInput;
        PlayerInput.OnJumpInput           -= HandleJumpInput;
        PlayerCombat.OnAttackStateChanged -= HandleAttackStateChanged;
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void HandleMoveInput(Vector2 input)
    {
        if (!canMove) return;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        if (input.magnitude < 0.1f)
        {
            moveDirection.x = moveDirection.z = 0f;

            animator.SetFloat("Speed",     0f, speedDampTime,     Time.deltaTime);
            animator.SetFloat("Direction", 0f, directionDampTime, Time.deltaTime);
            animator.SetBool ("IsRunning", false);

            OnMove?.Invoke(0f, 0f, isRunning);
            return;
        }
        
        Vector3 forward = cameraTransform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y   = 0f; right.Normalize();

        Vector3 movementDirection = (forward * input.y + right * input.x).normalized;
        moveDirection = new Vector3(
            movementDirection.x * speed,
            moveDirection.y,
            movementDirection.z * speed
        );
        
        bool isBackpedal = Vector3.Dot(transform.forward, movementDirection) < 0f;
        if (movementDirection.magnitude > 0f && !isBackpedal && !isJumping)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            float smoothed    = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothed, 0f);
        }

        animator.SetFloat("Speed",     movementDirection.magnitude, speedDampTime,     Time.deltaTime);
        animator.SetFloat("Direction", input.y,                       directionDampTime, Time.deltaTime);
        animator.SetBool ("IsRunning", isRunning);

        OnMove?.Invoke(input.x, input.y, isRunning);
    }

    private void HandleJumpInput()
    {
        if (characterController.isGrounded && !isJumping && canMove)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
            
            animator.SetTrigger("Jump");
            OnJump?.Invoke(true);
        }
    }

    private void Update()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
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
        animator.SetBool("IsAttacking", isAttacking);
    }
}
