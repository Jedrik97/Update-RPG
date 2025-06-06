using UnityEngine;
using TMPro;
using Zenject;
using System.Collections;

public class Vendor : MonoBehaviour
{
    [Header("UI")]
    public GameObject vendorUIPanel;
    public TextMeshProUGUI messageText;

    private readonly string defaultPrompt = "Press R, to bought Potion";
    private Coroutine revertCoroutine;
    private bool _playerInRange = false;
    private PlayerInventory _inventory;
    private int MaxPotions = 5;
    
    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    void Start()
    {
        if (vendorUIPanel != null)
            vendorUIPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            if (vendorUIPanel != null)
            {
                vendorUIPanel.SetActive(true);
                ShowDefaultPrompt();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!_playerInRange || !other.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            bool bought = _inventory.BuyHealthPotion();
            if (bought)
                ShowTemporaryMessage("You bought 1 Potion");
            else
                ShowTemporaryMessage(_inventory.HealthPotions >= MaxPotions
                    ? $"Maximum {MaxPotions} Potions"
                    : "Not enough gold");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (vendorUIPanel != null)
                vendorUIPanel.SetActive(false);
            if (revertCoroutine != null)
                StopCoroutine(revertCoroutine);
        }
    }

    private void ShowDefaultPrompt()
    {
        if (messageText != null)
            messageText.text = defaultPrompt;
        

    }

    private void ShowTemporaryMessage(string text)
    {
        if (messageText != null)
            messageText.text = text;
        else

        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);
        revertCoroutine = StartCoroutine(RevertToDefaultPromptAfterDelay(2f));
    }

    private IEnumerator RevertToDefaultPromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowDefaultPrompt();
        revertCoroutine = null;
    }
}