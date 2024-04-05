using System;
using BeABachelor.Networking.Interface;
using Zenject;

namespace BeABachelor.Networking.DI
{
    public class SynchronizationControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(ISynchronizationController), typeof(IInitializable), typeof(IDisposable))
                .To<SynchronizationController>()
                .FromNew()
                .AsSingle();
        }
    }
}