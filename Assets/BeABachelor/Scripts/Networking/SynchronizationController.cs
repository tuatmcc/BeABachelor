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
            _networkManager.SynchronizationController = null;
        }
    }
}