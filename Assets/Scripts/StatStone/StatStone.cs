using UnityEngine;
using Zenject;

public class StatStone : MonoBehaviour
{
    private int _keyDown = 0;

    public enum StatType { Strength, Stamina, Intelligence, Wisdom }
    public StatType statType;

    private PlayerStats _playerStats;
    [SerializeField] private StatUI statUI;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && statUI != null)
        {
            statUI.Show(_playerStats, statType.ToString());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.R))
        {
            _keyDown++;
            if (_keyDown == 1 && _playerStats != null)
            {
                _playerStats.SpendStatPoint(statType.ToString());
                statUI.Show(_playerStats, statType.ToString());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && statUI != null)
        {
            _keyDown = 0;
            statUI.Hide();
        }
    }
}