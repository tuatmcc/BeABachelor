using System;
using System.Net;
using BeABachelor.Networking.Play;
using Cysharp.Threading.Tasks;

namespace BeABachelor.Networking.Interface
{
    public interface INetworkManager
    {
        event Action<EndPoint> OnConnected;
        event Action<EndPoint> OnConnecting; 
        event Action OnDisconnected;
        bool IsConnected { get; }
        EndPoint RemoteEndPoint { get; }
        int ClientPort { get; }
        bool IsHost { get; }
        SynchronizationController SynchronizationController { get; set;}
        NetworkState NetworkState { get; }
        UniTask ConnectAsync(bool isHost, string ip, int remotePort = 8888, int clientPort = 8888, int timeOut = 5);
        void Disconnect();
    }
}