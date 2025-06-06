using UnityEngine;
using Zenject;

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
        Container.Bind<GameManager>()
            .FromComponentInHierarchy(GameManager)
            .AsSingle()
            .Lazy();
        Container.Bind<PauseMenuController>()
            .FromComponentInHierarchy(PauseMenuController)
            .AsSingle()
            .Lazy();
        Container.Bind<SlotSelectController>()
            .FromComponentInHierarchy(SlotSelectController)
            .AsSingle()
            .Lazy();
        Container.Bind<TemporaryMessageUI>()
            .FromComponentInHierarchy(TemporaryMessageUI)
            .AsSingle()
            .Lazy();
        
        var playerGO = Object.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerGO.SetActive(true);
        
        Container.Bind<PlayerStats>()
            .FromComponentOn(playerGO)
            .AsSingle();
        Container.Bind<HealthPlayerController>()
            .FromComponentOn(playerGO)
            .AsSingle();
        Container.Bind<CharacterController>()
            .FromComponentOn(playerGO)
            .AsSingle();
        Container.Bind<PlayerInventory>()
            .FromComponentOn(playerGO)
            .AsSingle();
        Container.Bind<InventoryUI>()
            .FromComponentOn(playerGO)
            .AsSingle();
        
        Container.InjectGameObject(playerGO);
        
        Container.BindInterfacesTo<SaveLoadInitializer>()
            .AsSingle()
            .NonLazy();
    }
}
