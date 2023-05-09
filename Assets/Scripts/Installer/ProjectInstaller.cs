using RodongSurviver.Manager;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameObject ItemDataContainer;

    public override void InstallBindings()
    {
        Container.Bind<GameManager>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .NonLazy();

        Container.Bind<SceneManager>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .NonLazy();

        Container.Bind<ItemDataContainer>()
            .FromComponentInNewPrefab(ItemDataContainer)
            .AsCached()
            .NonLazy();
    }
}