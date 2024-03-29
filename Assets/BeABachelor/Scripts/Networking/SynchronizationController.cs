using System;
using System.Collections.Generic;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class SynchronizationController : MonoBehaviour
    {
        [SerializeField] private List<MonoSynchronization> _monoSynchronizations;
        [Inject] private INetworkManager _networkManager;
        
        public List<MonoSynchronization> MonoSynchronizations => _monoSynchronizations;
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