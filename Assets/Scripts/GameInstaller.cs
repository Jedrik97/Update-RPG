using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Привязка инстансов PlayerStats и GameManager
        Container.Bind<PlayerStats>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        // Привязка Weapon (если нужно)
        Container.Bind<Weapon>().To<Weapon>().AsSingle();
    }
}