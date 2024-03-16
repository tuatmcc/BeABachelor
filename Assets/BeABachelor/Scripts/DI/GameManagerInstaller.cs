using System;
using Zenject;

namespace BeABachelor.DI
{
    public class GameManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(GameManager), typeof(IInitializable), typeof(IDisposable))
                .To<GameManager>()
                .FromNew()
                .AsSingle();
        }
    }
}
