using UnityEngine;
using Zenject;

namespace BeABachelor.Networking.DI
{
    public class SynchronizationControllerInstaller : MonoInstaller
    {
        [SerializeField] private SynchronizationController _synchronizationController;
        
        public override void InstallBindings()
        {
            Container.Bind<SynchronizationController>().FromInstance(_synchronizationController).AsSingle();
        }
    }
}