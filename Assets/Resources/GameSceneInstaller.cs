using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    public GameObject GameManager;
    public GameObject Player;
    public GameObject PauseMenuController;
    public GameObject SlotSelectController;

    public override void InstallBindings()
    {
        Container.Bind<GameManager>()
            .FromComponentInHierarchy(GameManager)
            .AsSingle()
            .Lazy();
        Container.Bind<PlayerStats>()
            .FromComponentInHierarchy(Player)
            .AsSingle()
            .Lazy();
        Container.Bind<HealthPlayerController>()
            .FromComponentInHierarchy(Player)
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
    }
}