using UnityEngine;
using Zenject;
using RodongSurviver.Manager;

namespace RodongSurviver.Installer
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Resolve<GameManager>();

            Container.Resolve<SceneManager>();
        }
    }
}