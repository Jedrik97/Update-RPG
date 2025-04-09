using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public GameObject GameManagerPrefab;
    public GameObject PlayerPrefab;
    
    public override void InstallBindings()
    {
        Container.Bind<GameManager>()
            .FromComponentInNewPrefab(GameManagerPrefab)
            .AsSingle()
            .Lazy();
        Container.Bind<PlayerStats>()
            .FromComponentInNewPrefab(PlayerPrefab)
            .AsSingle()
            .Lazy();
    }
}