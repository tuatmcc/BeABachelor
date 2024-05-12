using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Interface;
using BeABachelor.Networking.Config;
using BeABachelor.Networking.Interface;
using BeABachelor.Util;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BeABachelor.Networking
{
    public class NetworkManager : INetworkManager, IInitializable, IDisposable, IFixedTickable
    {
        private int _clientPort;
        private int _remoteEndpointPort;
        private UdpClient _client;
        private EndPoint _endpoint;
        private CancellationTokenSource _disposeCancellationTokenSource;
        private bool _isHost;
        private bool _opponentReady;
        private NetworkState _networkState;
        
        [Inject] private IGameManager _gameManager;

        public event Action OnSearching;
        public event Action OnConnecting;
        public event Action<EndPoint> OnConnected;
        public event Action OnDisconnected;
        public event Action OpponentReadyEvent;
        public event Action<NetworkState> OnNetworkStateChanged;
        public bool IsConnected => _networkState == NetworkState.Connected;
        public bool OpponentReady
        {
            get => _opponentReady;
            private set
            {
                if (!_opponentReady && value)
                    OpponentReadyEvent?.Invoke();
            }
        }
        public ISynchronizationController SynchronizationController { get; set; }
        public NetworkState NetworkState
        {
            get => _networkState;
            private set
            {
                Debug.Log($"NetworkState: {_networkState} -> {value}");
                switch (value)
                {
                    case NetworkState.Searching:
                        OnSearching?.Invoke();
                        _networkState = NetworkState.Searching;
                        break;
                    case NetworkState.Connecting:
                        OnConnecting?.Invoke();
                        _networkState = NetworkState.Connecting;
                        break;
                    case NetworkState.Connected:
                        OnConnected?.Invoke(_endpoint);
                        _networkState = NetworkState.Connected;
                        break;
                    case NetworkState.Disconnected:
                        OnDisconnected?.Invoke();
                        _networkState = NetworkState.Disconnected;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
        public async UniTask ConnectAsync(int timeOut = 5)
        {
            NetworkState = NetworkState.Searching;
            _client = new UdpClient(8888);
            
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var broadcastCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(broadcastCancellationTokenSource.Token, timeoutToken, _disposeCancellationTokenSource.Token).Token;
            UdpReceiveResult result;
            SearchPlayer(token);
            
            do
            {
                var broadcastReceiveTask = _client.ReceiveAsync();
                // ちょっと待たないと相手が受信できない
                // taskComplete tokenCancel
                // true         true        受信成功
                // true         false       受信成功
                // false        true        タイムアウトなど
                // false        false       未受診
                
                // 受信待ち
                await UniTask.WaitUntil(() => broadcastReceiveTask.IsCompleted || token.IsCancellationRequested);

                if (!broadcastReceiveTask.IsCompleted && timeoutToken.IsCancellationRequested)
                {
                    // タイムアウト
                    Debug.LogError("Connection timed out");
                    _client.Close();
                    // broadcastReceiveTask.Dispose();
                    broadcastCancellationTokenSource.Cancel();  // ブロードキャストを止める
                    NetworkState = NetworkState.Disconnected;
                    return;
                }

                // 受信成功
                result = broadcastReceiveTask.Result;
                
                // 有効なデータでないときは再受信する
            }while (!ValidAck(result));

            // ちょっと待たないと相手が受信できない
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            broadcastCancellationTokenSource.Cancel();  // ブロードキャストを止める
            
            _endpoint = result.RemoteEndPoint;
            _client.Connect((IPEndPoint) _endpoint);
            NetworkState = NetworkState.Connecting;
            
            bool success;
            if (IsWaitNegotiation((IPEndPoint)_endpoint))
            {
                Debug.Log("Receive negotiation");
                success = await ReceiveNegotiationAsync(timeOut);
            }
            else
            {
                Debug.Log("Send negotiation");
                success = await SendNegotiationAsync(timeOut);
            }
            
            if (!success)
            {
                Debug.LogError("Connection failed");
                _client.Close();
                NetworkState = NetworkState.Disconnected;
                return;
            }
            
            NetworkState = NetworkState.Connected;
            
            ReceiveTask().Forget();
        }

        private void SearchPlayer(CancellationToken token)
        {
            Observable.Interval(TimeSpan.FromSeconds(0.5f), token)
                .Subscribe(_ =>
                {
                    foreach (var ip in JsonConfigure.NetworkConfig.ipAddresses)
                    {
                        Debug.Log($"Send broadcast to {ip}");
                        _client.Send(new byte[] { 0x01 }, 1, ip, 8888);
                    }
                });
            // _client.Connect(IPAddress.Broadcast, 8888);
            // _client.EnableBroadcast = true;
            // Debug.Log(_client.Client.LocalEndPoint);
            // Observable.Interval(TimeSpan.FromSeconds(0.5f), token)
            //     .Subscribe(_ => _client.Send(new byte[] { 0x01 }, 1));
        }
        
        private bool ValidAck(UdpReceiveResult result)
        {
            return result.Buffer.Length == 1 && result.Buffer[0] == 0x01;
        }
        
        private bool IsWaitNegotiation(IPEndPoint endPoint)
        {
            var myIp = BitConverter.ToUInt32(((IPEndPoint)_client.Client.LocalEndPoint).Address.GetAddressBytes().Reverse().ToArray(), 0);
            var opponentIp = BitConverter.ToUInt32(endPoint.Address.GetAddressBytes().Reverse().ToArray(), 0);
            Debug.Log($"IsWaitNegotiation My IP: {myIp}, Opponent IP: {opponentIp} {((IPEndPoint)_client.Client.LocalEndPoint).Address}");
            return opponentIp < myIp;
        }

        private async UniTask<bool> SendNegotiationAsync(int timeOut)
        {
            var random = Random.Range(0, 2);
            await _client.SendAsync(new byte[] { 0x02, (byte)random }, 2);
            _gameManager.PlayerType = random == 0 ? PlayerType.Kouken : PlayerType.Hakken;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut + 3));
            var negotiationCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(negotiationCancellationTokenSource.Token, timeoutToken, _disposeCancellationTokenSource.Token).Token;
            UdpReceiveResult result;
            do
            {
                var negotiationReceiveTask = _client.ReceiveAsync();
                await UniTask.WaitUntil(() => negotiationReceiveTask.IsCompleted, cancellationToken: token);
                if (!negotiationReceiveTask.IsCompleted && timeoutToken.IsCancellationRequested)
                {
                    // タイムアウト
                    Debug.LogError("Connection timed out");
                    _client.Close();
                    negotiationReceiveTask.Dispose();
                    negotiationCancellationTokenSource.Cancel();
                    return false;
                }
                
                result = negotiationReceiveTask.Result;
            }while (!ValidNegotiation(result.Buffer));
            
            negotiationCancellationTokenSource.Cancel();
            return true;
        }

        private async UniTask<bool> ReceiveNegotiationAsync(int timeOut)
        {
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut + 3));
            var negotiationCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(negotiationCancellationTokenSource.Token,
                timeoutToken, _disposeCancellationTokenSource.Token).Token;
            UdpReceiveResult result;

            do
            {
                var negotiationReceiveTask = _client.ReceiveAsync();
                await UniTask.WaitUntil(() => negotiationReceiveTask.IsCompleted || token.IsCancellationRequested);
                if (!negotiationReceiveTask.IsCompleted && timeoutToken.IsCancellationRequested)
                {
                    // タイムアウト
                    Debug.LogError("Connection timed out");
                    _client.Close();
                    negotiationReceiveTask.Dispose();
                    negotiationCancellationTokenSource.Cancel();
                    return false;
                }

                result = negotiationReceiveTask.Result;
            }while (!ValidNegotiation(result.Buffer));
            
            negotiationCancellationTokenSource.Cancel();
            _gameManager.PlayerType = result.Buffer[1] == 0 ? PlayerType.Hakken : PlayerType.Kouken;
            await _client.SendAsync(new byte[] { 0x02, result.Buffer[1] }, 2);
            return true;
        }

        private static bool ValidNegotiation(IReadOnlyList<byte> data)
        {
            return data.Count == 2 && data[0] == 0x02;
        }

        private async UniTask ReceiveTask()
        {
            Debug.Log("ReceiveTask start");
            while (IsConnected)
            {
                var task = _client.ReceiveAsync();
                await UniTask.WaitUntil(() =>
                    task.IsCompleted || !IsConnected || _disposeCancellationTokenSource.Token.IsCancellationRequested);
                if (!task.IsCompleted)
                {
                    Debug.Log("ReceiveTask is cancelled");
                    return;
                }
                
                if (_disposeCancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.Log("ReceiveTask is cancelled");
                    return;
                }

                var reader = new BinaryReader(new MemoryStream(task.Result.Buffer));
                if (reader.ReadByte() != 0xaa) continue;
                OpponentReady = true;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var hashCode = reader.ReadInt32();
                    var length = reader.ReadInt32();
                    var data = reader.ReadBytes(length);
                    var synchronization = SynchronizationController.MonoSynchronizations[hashCode];
                    if (synchronization == null) continue;
                    synchronization.FromBytes(data);
                }
            }
        }
        public void Initialize()
        {
            _disposeCancellationTokenSource = new CancellationTokenSource();
            NetworkState = NetworkState.Disconnected;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _disposeCancellationTokenSource?.Cancel();
            NetworkState = NetworkState.Disconnected;
        }
        
        public void Disconnect()
        {
            UniTask.Create(async () =>
            {
                await UniTask.Delay(1000);
                _client.Close();
                NetworkState = NetworkState.Disconnected;
                OpponentReady = false;
                return UniTask.CompletedTask;
            });
        }

        public void FixedTick()
        {
            if (!IsConnected || SynchronizationController == null) return;
            var writer = new BinaryWriter(new MemoryStream());
            // 0xaa はプレイ中
            writer.Write((byte) 0xaa);
            foreach (var synchronization in SynchronizationController.MonoSynchronizations)
            {
                var monoSynchronization = synchronization.Value;
                var hashCode = monoSynchronization.GetHashCode();
                var data = monoSynchronization.ToBytes();
                writer.Write(hashCode);
                writer.Write(data.Length);
                writer.Write(data);
            }
            _client.Send(((MemoryStream)writer.BaseStream).ToArray(), (int)writer.BaseStream.Length);
        }
    }
}