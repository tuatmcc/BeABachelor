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
        event Action OpponentReadyEvent; 
        bool IsConnected { get; }
        EndPoint RemoteEndPoint { get; }
        int ClientPort { get; }
        bool IsHost { get; }
        bool OpponentReady { get; set; }
        ISynchronizationController SynchronizationController { get; set;}
        NetworkState NetworkState { get; }
        UniTask ConnectAsync(bool isHost, string ip, int remotePort = 8888, int clientPort = 8888, int timeOut = 5);
        UniTask ConnectAsync(int timeOut = 5);
        void SetRemoteEndPointAndClientPort(bool isHost, string ip, int remotePort, int clientPort);
        void Disconnect();
    }
}