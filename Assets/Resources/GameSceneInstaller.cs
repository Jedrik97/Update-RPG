using UnityEngine;
using Zenject;
using Unity.Cinemachine;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Player Setup")]
    public GameObject playerPrefab;

    public Transform spawnPoint;

    [Header("Other Controllers")]
    public GameObject GameManager;

    public GameObject PauseMenuController;
    
    public GameObject SlotSelectController;

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
        
        Container.InjectGameObject(playerGO);
    }
}