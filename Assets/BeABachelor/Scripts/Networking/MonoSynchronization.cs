using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public abstract class MonoSynchronization : MonoBehaviour, IBinariable
    {
        [Inject] protected INetworkManager _networkManager;
        // ホストに合わせます。 False の場合、ホストがクライアントに合わせます。
        [SerializeField] public bool Match2Host = true;
        public abstract byte[] ToBytes();
        public abstract void FromBytes(byte[] bytes);
    }
}