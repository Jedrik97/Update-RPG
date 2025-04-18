using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class SceneInstaller : MonoInstaller
{
    public GameObject GameManager;
    public GameObject Player;
    
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
    }
}