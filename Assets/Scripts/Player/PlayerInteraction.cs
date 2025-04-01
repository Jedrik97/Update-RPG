using UnityEngine;
using Zenject;
public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 0.5f;
    public KeyCode interactKey = KeyCode.R;

    private IPlayerStatsManager playerStatsManager;

    [Inject]
    public void Construct(IPlayerStatsManager playerStatsManager)
    {
        this.playerStatsManager = playerStatsManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            InteractWithNearbyStone();
        }
    }

    void InteractWithNearbyStone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            StatStone stone = hitCollider.GetComponent<StatStone>();
            if (stone != null)
            {
                if (playerStatsManager != null)
                {
                    playerStatsManager.SpendStatPoint(stone.statType.ToString());
                }
            }
        }
    }
}