using UnityEngine;
using Zenject;
using UnityEngine.AI;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Player Setup")]
    public GameObject playerPrefab;
    public Transform spawnPoint;

    [Header("Other Controllers")]
    public GameObject GameManager;
    public GameObject PauseMenuController;
    public GameObject SlotSelectController;
    public GameObject TemporaryMessageUI;

    public override void InstallBindings()
    {
        // Bind core controllers from scene
        Container.Bind<GameManager>().FromComponentInHierarchy(GameManager).AsSingle().Lazy();
        Container.Bind<PauseMenuController>().FromComponentInHierarchy(PauseMenuController).AsSingle().Lazy();
        Container.Bind<SlotSelectController>().FromComponentInHierarchy(SlotSelectController).AsSingle().Lazy();
        Container.Bind<TemporaryMessageUI>().FromComponentInHierarchy(TemporaryMessageUI).AsSingle().Lazy();

        // Instantiate the player prefab manually
        var playerGO = Object.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerGO.SetActive(true);

        // Initialize NavMeshAgent if present
        var nav = playerGO.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.enabled = true;
            nav.Warp(spawnPoint.position);
        }

        // Bind player-specific components BEFORE injection
        Container.Bind<PlayerStats>().FromComponentOn(playerGO).AsSingle();
        Container.Bind<HealthPlayerController>().FromComponentOn(playerGO).AsSingle();
        Container.Bind<CharacterController>().FromComponentOn(playerGO).AsSingle();
        Container.Bind<PlayerInventory>().FromComponentOn(playerGO).AsSingle();
        Container.Bind<InventoryUI>().FromComponentOn(playerGO).AsSingle();

        // Inject dependencies into the instantiated player GameObject
        Container.InjectGameObject(playerGO);

        // Initialize save/load
        Container.BindInterfacesTo<SaveLoadInitializer>().AsSingle().NonLazy();
    }
}
