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
    
    private Dictionary<string, float> attackDurations = new Dictionary<string, float>
    {
        { "Attack1", 1.15f },
        { "Attack2", 1.20f },
        { "Attack3", 1.22f }, 
        { "Attack4", 2.13f },
        { "Attack5", 2.10f } 
    };

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    [Inject]
    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    void Update()
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
        yield return new WaitForSeconds(0.1f);

        if (weapon != null)
        {
            weapon.EnableCollider(true);
        }
        else
        {
            Debug.LogError("weapon не назначен!");
        }
        
        float attackDuration = attackDurations[attackName];
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        OnAttackStateChanged?.Invoke(false);
    }
}
