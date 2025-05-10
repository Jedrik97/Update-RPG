using System;
using UnityEngine;
using Zenject;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<bool> OnAttackStateChanged;
    public static event Action OnAoeTriggered;

    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;

    private bool isAttacking = false;
    private PlayerStats playerStats;
    private GameManager gameManager;

    [Inject]
    public void Construct(PlayerStats playerStats, GameManager gameManager)
    {
        this.playerStats = playerStats;
        this.gameManager = gameManager;
    }

    private void Update()
    {
        // если не атакуем — стартуем новую атаку
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartAttack("Attack1");
            if (Input.GetKeyDown(KeyCode.Alpha2)) StartAttack("Attack2");
            if (Input.GetKeyDown(KeyCode.Alpha3)) StartAttack("Attack3");
            if (Input.GetKeyDown(KeyCode.Alpha4)) StartAttack("Attack4");
            if (Input.GetKeyDown(KeyCode.Alpha5)) StartAttack("Attack5");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha2)) animator.SetTrigger("Attack2");
            if (Input.GetKeyDown(KeyCode.Alpha3)) animator.SetTrigger("Attack3");
            if (Input.GetKeyDown(KeyCode.Alpha4)) animator.SetTrigger("Attack4");
            if (Input.GetKeyDown(KeyCode.Alpha5)) animator.SetTrigger("Attack5");
        }

        if (Input.GetKeyDown(KeyCode.F))
            OnAoeTriggered?.Invoke();
    }

    
    private void StartAttack(string triggerName)
    {
        isAttacking = true;
        OnAttackStateChanged?.Invoke(true);
        animator.ResetTrigger(triggerName);  
        animator.SetTrigger(triggerName);
    }
    
    public void OnAttackEnded()
    {
        isAttacking = false;
        OnAttackStateChanged?.Invoke(false);
    }

    public void EnableWeaponHitbox()
    {
        weapon?.EnableCollider(true);
    }

    public void DisableWeaponHitbox()
    {
        weapon?.EnableCollider(false);
    }
}
