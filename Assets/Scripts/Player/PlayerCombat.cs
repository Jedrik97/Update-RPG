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
    private bool canComboWindow = false;
    private int currentComboStep = 0;

    private PlayerStats playerStats;
    private GameManager gameManager;

    [Inject]
    public void Construct(PlayerStats ps, GameManager gm)
    {
        playerStats = ps;
        gameManager   = gm;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnAoeTriggered?.Invoke();
            return;
        }
        
        if (!isAttacking)
        {
            TryStartAttack();
        }
        else if (canComboWindow)
        {
            TryQueueNextAttack();
        }
    }

    private void TryStartAttack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TriggerAttack(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TriggerAttack(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TriggerAttack(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TriggerAttack(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TriggerAttack(5);
    }

    private void TryQueueNextAttack()
    {
        int next = currentComboStep + 1;
        if (next > 5) return; 
        
        if (Input.GetKeyDown(KeyCode.Alpha0 + next))
        {
            TriggerAttack(next);
            canComboWindow = false;
        }
    }

    private void TriggerAttack(int step)
    {
        animator.SetTrigger($"Attack{step}");

        isAttacking      = true;
        currentComboStep = step;
        OnAttackStateChanged?.Invoke(true);
    }
    
    public void EnableComboWindow()  
    {
        canComboWindow = true;
    }
    
    public void DisableComboWindow()
    {
        canComboWindow = false;
    }
    
    public void EndAttack()
    {
        isAttacking      = false;
        currentComboStep = 0;
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
