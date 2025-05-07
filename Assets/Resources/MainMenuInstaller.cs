using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public GameObject MainMenuController;
    public GameObject SlotSelectController;

    public override void InstallBindings()
    {
        Container.Bind<MainMenuController>()
            .FromComponentInHierarchy(MainMenuController)
            .AsSingle()
            .Lazy();
        Container.Bind<SlotSelectController>()
            .FromComponentInHierarchy(SlotSelectController)
            .AsSingle()
            .Lazy();
    }
}