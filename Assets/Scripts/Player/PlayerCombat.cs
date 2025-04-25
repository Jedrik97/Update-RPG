// PlayerCombat.cs
using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private readonly Dictionary<string, float> attackDurations = new Dictionary<string, float>
    {
        { "Attack1", 1.15f },
        { "Attack2", 1.20f },
        { "Attack3", 1.22f }, 
        { "Attack4", 2.13f },
        { "Attack5", 2.10f } 
    };

    [Inject]
    public void Construct(PlayerStats playerStats, GameManager gameManager)
    {
        this.playerStats = playerStats;
        this.gameManager = gameManager;
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(PerformAttack("Attack1"));
            if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(PerformAttack("Attack2"));
            if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(PerformAttack("Attack3"));
            if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(PerformAttack("Attack4"));
            if (Input.GetKeyDown(KeyCode.Alpha5)) StartCoroutine(PerformAttack("Attack5"));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            OnAoeTriggered?.Invoke();
        }
    }

    private IEnumerator PerformAttack(string attackName)
    {
        isAttacking = true;
        OnAttackStateChanged?.Invoke(true);

        animator.Play(attackName);
        
        if (attackDurations.TryGetValue(attackName, out float duration))
            yield return new WaitForSeconds(duration);
        else
            yield return null;

        isAttacking = false;
        OnAttackStateChanged?.Invoke(false);
    }
    
    public void EnableWeaponHitbox()
    {
        if (weapon)
            weapon.EnableCollider(true);
    }

    public void DisableWeaponHitbox()
    {
        if (weapon)
            weapon.EnableCollider(false);
    }
}
