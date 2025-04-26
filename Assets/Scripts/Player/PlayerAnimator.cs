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
        float speedMultiplier = running ? 1f : 0.5f;
        Vector2 move = new Vector2(x, z);
        float speed = move.magnitude * speedMultiplier;
        animator.SetFloat("Speed", speed);
        
        if (move.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg;
            animator.SetFloat("Direction", angle);
        }
        
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