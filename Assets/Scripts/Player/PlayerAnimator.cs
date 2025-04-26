using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

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
        const float deadZone = 0.1f;
        Vector2 input = new Vector2(x, z);

        // Если почти не движемся — встанем в Idle
        if (input.magnitude < deadZone)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 0f);
        }
        else
        {
            // MoveZ: +1 — вперёд, –1 — назад
            float moveZ = Mathf.Sign(z);

            // MoveX: –1 — ходьба, +1 — бег
            float moveX = running ? 1f : -1f;

            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveZ", moveZ);
        }

        // Сбрасываем флаг прыжка при ходьбе/беге
        animator.SetBool("IsJumping", false);
    }

    private void HandleJump(bool isJumping)
    {
        if (isJumping)
        {
            animator.SetBool("IsJumping", true);
            animator.SetTrigger("Jump");
        }
    }

    private void OnDisable()
    {
        PlayerMovement.OnMove -= HandleMovement;
        PlayerMovement.OnJump -= HandleJump;
    }
}