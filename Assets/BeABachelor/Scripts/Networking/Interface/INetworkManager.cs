using System;
using System.Net;
using Cysharp.Threading.Tasks;

namespace BeABachelor.Networking.Interface
{
    public interface INetworkManager
    {
        event Action<EndPoint> OnConnected;
        event Action OnDisconnected;
        event Action<byte[]> OnReceived;
        bool IsConnected { get; }
        EndPoint RemoteEndPoint { get; }
        int ClientPort { get; }
        UniTask ConnectAsync(int timeOut = 5);
        UniTask SendAsync(IBinariable binariable);
        void Disconnect();
    }
}