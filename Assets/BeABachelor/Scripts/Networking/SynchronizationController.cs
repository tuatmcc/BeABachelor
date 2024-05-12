using System;
using System.Collections.Generic;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class SynchronizationController : ISynchronizationController, IInitializable, IDisposable
    { 
        private List<MonoSynchronization> _monoSynchronizations;
        [Inject] private INetworkManager _networkManager;
        
        public List<MonoSynchronization> MonoSynchronizations => _monoSynchronizations;
        
        public void Initialize()
        {
            _monoSynchronizations = new List<MonoSynchronization>();
            _networkManager.SynchronizationController = this;
        }

        public void Register(MonoSynchronization monoSynchronization)
        {
            _monoSynchronizations.Add(monoSynchronization);
        }

        public void Dispose()
        {
            foreach (var monoSynchronization in _monoSynchronizations)
            {
                switch (monoSynchronization)
                {
                    case ScrollItemSynchronization scrollItemSynchronization:
                        Debug.Log("ScrollItemSynchronization");
                        break;
                    case TransformSynchronization transformSynchronization:
                        Debug.Log("TransformSynchronization");
                        break;
                    case PlayerAnimationSynchronization playerAnimationSynchronization:
                        Debug.Log("PlayerAnimationSynchronization");
                        break;
                }
            }
            _networkManager.SynchronizationController = null;
        }
    }
}