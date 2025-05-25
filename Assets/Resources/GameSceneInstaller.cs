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
        // 1) Биндим все внешние контроллеры
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
        
        // 2) Инстанцируем игрока и биндим его компоненты
        var playerGO = Object.Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
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
        
        // 3) Биндим ваш InventoryUI (он уже есть в сцене!)
        Container.Bind<InventoryUI>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
        
        // 4) Наконец, регистрируем IInitializable для загрузки сохранений
        Container.BindInterfacesTo<SaveLoadInitializer>()
            .AsSingle()
            .NonLazy();
    }
}
