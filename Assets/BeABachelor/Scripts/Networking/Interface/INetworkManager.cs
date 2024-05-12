using System;
using System.Net;
using BeABachelor.Networking.Play;
using Cysharp.Threading.Tasks;

namespace BeABachelor.Networking.Interface
{
    public interface INetworkManager
    {
        event Action OnSearching;
        event Action OnConnecting;
        event Action<EndPoint> OnConnected;
        event Action OnDisconnected;
        event Action OpponentReadyEvent; 
        event Action<NetworkState> OnNetworkStateChanged;
        bool IsConnected { get; }
        bool OpponentReady { get; }
        ISynchronizationController SynchronizationController { get; set;}
        NetworkState NetworkState { get; }
        UniTask ConnectAsync(int timeOut = 5);
        void Disconnect();
    }
}