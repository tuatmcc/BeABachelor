using System;
using BeABachelor.Networking.Interface;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BeABachelor.Networking
{
    public abstract class MonoSynchronization : MonoBehaviour, IBinariable
    {
        [Inject] protected INetworkManager _networkManager;
        [Inject] protected ISynchronizationController _synchronizationController;

        public abstract int ObjectID
        {
            get;
        }

        protected void Start()
        {
            _synchronizationController.Register(this);
        }

        // ホストに合わせます。 False の場合、ホストがクライアントに合わせます。
        [SerializeField] public bool UseReceivedData = true;
        public abstract byte[] ToBytes();
        public abstract void FromBytes(byte[] bytes);
    }
}