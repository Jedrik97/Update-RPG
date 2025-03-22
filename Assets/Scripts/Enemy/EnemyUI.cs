using UnityEngine;
using System.Collections;

public class EnemyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMPro.TMP_Text enemyNameText;
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private UnityEngine.UI.Image enemyCircle;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private Transform player;
    
    private EnemyBase enemyBase;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        if (enemyBase)
        {
            enemyBase.OnHealthChanged += UpdateHealthUI;
            enemyBase.OnDeath += HideUI;
        }

        InitializeUI();
        StartCoroutine(DelayedHideUI());
    }

    private IEnumerator DelayedHideUI()
    {
        yield return new WaitForEndOfFrame();
        HideUI();
    }

    private void Update()
    {
        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= detectionRange)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }
    }

    private void OnDestroy()
    {
        if (enemyBase)
        {
            enemyBase.OnHealthChanged -= UpdateHealthUI;
            enemyBase.OnDeath -= HideUI;
        }
    }

    private void InitializeUI()
    {
        if (enemyNameText)
        {
            enemyNameText.text = enemyBase.enemyName;
        }

        if (healthBar)
        {
            healthBar.maxValue = enemyBase.maxHealth;
            healthBar.value = enemyBase.currentHealth;
        }
    }

    public void UpdateHealthUI(float currentHealth)
    {
        if (healthBar)
        {
            healthBar.value = currentHealth;
        }
    }

    public void ShowUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(true);
        if (healthBar) healthBar.gameObject.SetActive(true);
        if (enemyCircle) enemyCircle.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(false);
        if (healthBar) healthBar.gameObject.SetActive(false);
        if (enemyCircle) enemyCircle.gameObject.SetActive(false);
    }
}
