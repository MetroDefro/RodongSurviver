using UnityEngine;
using Zenject;

namespace RodongSurviver.Installer
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameManager>() // ContractType: 주입되는 Type
                .To<GameManager>() // ResultType: 바인딩할 Type (주입된 Type과 같거나, 파생)
                .FromNewComponentOnNewGameObject()
                .AsCached() // AsScope(): 재사용 빈도 혹은 여부 ... ContractType이 요청될 때마다 동일한 ResultType 인스턴스 재사용.
                .NonLazy();

            Container.Bind<SceneManager>()
                .FromNewComponentOnNewGameObject()
                .AsCached()
                .NonLazy();
        }
    }
}