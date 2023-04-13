using UnityEngine;
using Zenject;

namespace RodongSurviver.Installer
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameManager>() // ContractType: ���ԵǴ� Type
                .To<GameManager>() // ResultType: ���ε��� Type (���Ե� Type�� ���ų�, �Ļ�)
                .FromNewComponentOnNewGameObject()
                .AsCached() // AsScope(): ���� �� Ȥ�� ���� ... ContractType�� ��û�� ������ ������ ResultType �ν��Ͻ� ����.
                .NonLazy();

            Container.Bind<SceneManager>()
                .FromNewComponentOnNewGameObject()
                .AsCached()
                .NonLazy();
        }
    }
}