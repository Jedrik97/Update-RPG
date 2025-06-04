using System;
using UnityEngine;
using Zenject;

public class PlayerCombat : MonoBehaviour
{
    // Подписчики могут заблокировать/разблокировать движение
    public static event Action<bool> OnAttackStateChanged;
    public static event Action OnAoeTriggered;
    public static event Action<int> OnComboStepChanged;

    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;

    // Флаги комбо
    private bool isAttacking = false;      // Игрок сейчас в «активной» фазе удара?
    private bool canComboWindow = false;   // Можно ли принять ввод для следующего удара?
    private int currentComboStep = 0;      // Номер текущей атаки (1..5)
    private int queuedComboStep = 0;       // Если нажали вовремя, сохраняем сюда следующий (step+1)

    private PlayerStats playerStats;
    private GameManager gameManager;

    [Inject]
    public void Construct(PlayerStats ps, GameManager gm)
    {
        playerStats = ps;
        gameManager = gm;
    }

    private void Update()
    {
        // Пример: AOE-атака по F (не связана с комбо)
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnAoeTriggered?.Invoke();
            return;
        }

        // Если сейчас нет «активного» этапа удара — можно начать любой из 1..5
        if (!isAttacking)
        {
            for (int step = 1; step <= 5; step++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + step))
                {
                    Debug.Log($"[PlayerCombat] Start Attack{step}");
                    TriggerAttack(step);
                    break;
                }
            }
        }
        else
        {
            // Мы уже в каком-то «активном» ударе → слушаем только в окне ко́мбо
            if (canComboWindow)
            {
                int nextStep = currentComboStep + 1;
                if (nextStep <= 5 && Input.GetKeyDown(KeyCode.Alpha0 + nextStep))
                {
                    Debug.Log($"[PlayerCombat] Queued Attack{nextStep}");
                    queuedComboStep = nextStep;
                    // Закрываем окно, чтобы более не принимать лишний ввод
                    canComboWindow = false;
                }
            }
        }
    }

    /// <summary>
    /// Запустить конкретный шаг удара step (1..5). 
    /// Если это продолжение (chaining), перед этим явно сбросим старое isAttacking.
    /// </summary>
    private void TriggerAttack(int step)
    {
        // Установим триггер нужного шага
        animator.SetTrigger($"Attack{step}");

        // Если мы «перепрыгиваем» из предыдущего шага (queuedComboStep != 0),
        // то до вызова новой анимации сразу сбросим старый флаг isAttacking
        // и уведомим подписчиков, что «предыдущая» фаза закончилась.
        //
        // Трюк: queuedComboStep хранит шаг, который мы собираемся запустить
        // по цепочке. Если queuedComboStep совпал с step, значит мы сюда попали
        // в результате DisableComboWindow()->TriggerAttack(queuedComboStep). 
        // Значит нужно разлочить старый isAttacking.
        if (queuedComboStep == step)
        {
            // Сбросим флаг «мы в атаке» от предыдущего шага
            isAttacking = false;
            OnAttackStateChanged?.Invoke(false);
        }

        // Теперь установим текущий шаг (он станет «активным»).
        currentComboStep = step;
        queuedComboStep = 0;
        canComboWindow = false;

        // Разблокируем «isAttacking» и оповестим, что атака началась
        isAttacking = true;
        Debug.Log($"[PlayerCombat] Triggered Attack{step}");
        OnAttackStateChanged?.Invoke(true);
        OnComboStepChanged?.Invoke(step);
    }

    /// <summary>
    /// Animation Event: «середина» клипа AttackN — теперь можно принять ввод N+1.
    /// </summary>
    public void EnableComboWindow()
    {
        canComboWindow = true;
        Debug.Log($"[PlayerCombat] EnableComboWindow at Attack{currentComboStep}");

        // Если игрок успел нажать «следующее» до Enable (редкий случай),
        // queuedComboStep уже мог быть равен current+1 — цепляем комбо сразу:
        if (queuedComboStep == currentComboStep + 1)
        {
            Debug.Log($"[PlayerCombat] Immediate chaining to Attack{queuedComboStep}");
            // Обратите внимание: queuedComboStep == step, значит в TriggerAttack
            // он будет сбрасывать old isAttacking и запускать новый шаг.
            TriggerAttack(queuedComboStep);
        }
    }

    /// <summary>
    /// Animation Event: «ближе к концу» клипа AttackN — окно ввода закрыто.
    /// Если queuedComboStep == current+1, переход в следующий шаг.
    /// </summary>
    public void DisableComboWindow()
    {
        canComboWindow = false;
        Debug.Log($"[PlayerCombat] DisableComboWindow at Attack{currentComboStep}");

        if (queuedComboStep == currentComboStep + 1)
        {
            Debug.Log($"[PlayerCombat] Chaining to Attack{queuedComboStep}");
            // Сразу запустим новый шаг — см. TriggerAttack: он сбросит
            // старый isAttacking перед вызовом новой анимации.
            TriggerAttack(queuedComboStep);
        }
        // Если queuedComboStep != current+1, ничего не делаем —
        // тогда клип дойдёт до EndAttack() (Event в конце) и сбросит флаги.
    }

    /// <summary>
    /// Animation Event: «финал» шага AttackN, если комбо не продолжилось.
    /// Сброс всех флагов, возвращаемся в Idle.
    /// </summary>
    public void EndAttack()
    {
        Debug.Log($"[PlayerCombat] EndAttack from Attack{currentComboStep}");
        isAttacking = false;
        currentComboStep = 0;
        queuedComboStep = 0;
        canComboWindow = false;
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
