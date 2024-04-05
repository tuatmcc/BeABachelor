using System;
using System.Collections.Generic;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class SynchronizationController : MonoBehaviour
    {
        private List<MonoSynchronization> _monoSynchronizations;
        [Inject] private INetworkManager _networkManager;
        
        public List<MonoSynchronization> MonoSynchronizations => _monoSynchronizations;
        
        public void Register(MonoSynchronization monoSynchronization)
        {
            _monoSynchronizations.Add(monoSynchronization);
        }
        
        private void Start()
        {
            _networkManager.SynchronizationController = this;
        }

        private void OnDestroy()
        {
            _networkManager.SynchronizationController = null;
        }
    }
}