using UnityEngine;
using Zenject;

public class StatStone : MonoBehaviour
{
    private int _keyDown = 0;
    public enum StatType { Strength, Stamina, Intelligence, Wisdom }
    
    public StatType statType;

    private PlayerStats _playerStats;
    
    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    private void OnCollisionStay(Collision other)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _keyDown++;
            if (_keyDown == 1)
            {
                if (_playerStats)
                {
                    _playerStats.SpendStatPoint(statType.ToString());
                }
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        _keyDown = 0;
    }
}