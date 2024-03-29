using System;
using BeABachelor.Networking.Interface;
using BeABachelor.Networking.Play;
using Zenject;

namespace BeABachelor.Networking.DI
{
    public class NetworkManagerInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(INetworkManager), typeof(IInitializable), typeof(IDisposable), typeof(IFixedTickable))
                .To<NetworkManager>()
                .FromNew()
                .AsSingle();
        }
    }
}