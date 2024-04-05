using BeABachelor.Networking.Interface;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BeABachelor.Networking
{
    public abstract class MonoSynchronization : MonoBehaviour, IBinariable
    {
        [Inject] private SynchronizationController _synchronizationController;
        private void Awake()
        {
            _synchronizationController.Register(this);
        }
        
        [Inject] protected INetworkManager _networkManager;
        // ホストに合わせます。 False の場合、ホストがクライアントに合わせます。
        [SerializeField] public bool UseReceivedData = true;
        public abstract byte[] ToBytes();
        public abstract void FromBytes(byte[] bytes);
    }
}