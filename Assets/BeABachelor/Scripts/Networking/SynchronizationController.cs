using System;
using System.Collections;
using System.Collections.Generic;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class SynchronizationController : ISynchronizationController, IInitializable, IDisposable
    { 
        private Dictionary<int, MonoSynchronization> _monoSynchronizations;
        [Inject] private INetworkManager _networkManager;
        
        public Dictionary<int, MonoSynchronization> MonoSynchronizations => _monoSynchronizations;
        
        public void Initialize()
        {
            _monoSynchronizations = new Dictionary<int, MonoSynchronization>();
            _networkManager.SynchronizationController = this;
        }

        public void Register(MonoSynchronization monoSynchronization)
        {
            try
            {
                _monoSynchronizations.Add(monoSynchronization.GetHashCode(), monoSynchronization);
            }
            catch(Exception e)
            {
                switch (monoSynchronization)
                {
                    case ScrollItemSynchronization scrollItemSynchronization:
                        Debug.LogError($"{e} ScrollItemSynchronization");
                        break;
                    case TransformSynchronization transformSynchronization:
                        Debug.LogError($"{e} TransformSynchronization");
                        break;
                    case PlayerAnimationSynchronization playerAnimationSynchronization:
                        Debug.LogError($"{e} PlayerAnimationSynchronization");
                        break;
                }
            }
        }

        public void Dispose()
        {
            foreach (var monoSynchronization in _monoSynchronizations.Values)
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