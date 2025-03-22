using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private bool isRunning = false; // Следим за бегом

    private void OnEnable()
    {
        PlayerMovement.OnMove += HandleMovement;
        PlayerMovement.OnJump += HandleJump;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void HandleMovement(float x, float z, bool running)
    {
        isRunning = running;
        animator.SetBool("IsRunning", isRunning); // Устанавливаем состояние бега

        if (isRunning)
        {
            animator.SetFloat("RunX", x);
            animator.SetFloat("RunZ", z);
        }
        else
        {
            animator.SetFloat("WalkX", x);
            animator.SetFloat("WalkZ", z);
        }

        animator.SetBool("Idle", x == 0 && z == 0);
    }

    private void HandleJump(bool isJumping)
    {
        if (isJumping)
        {
            if (isRunning)
                animator.SetTrigger("JumpRun"); // Запускаем анимацию прыжка в беге
            else
                animator.SetTrigger("Jump"); // Запускаем обычный прыжок
        }
    }

    private void OnDisable()
    {
        PlayerMovement.OnMove -= HandleMovement;
        PlayerMovement.OnJump -= HandleJump;
    }
}