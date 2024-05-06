using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using BeABachelor.Networking.Serializable;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BeABachelor.Networking
{
    public class NetworkManager : INetworkManager, IInitializable, IDisposable, IFixedTickable
    {
        [Inject] IGameManager _gameManager;
        
        private string _ip;
        private int _clientPort;
        private int _remoteEndpointPort;
        private UdpClient _client;
        private bool _isConnected;
        private EndPoint _endpoint;
        private CancellationTokenSource _disposeCancellationTokenSource;
        private bool _isHost;
        private bool _opponentReady;
        private NetworkState _networkState;
        private int _randNum;
        private NetworkConfig _networkConfig;

        public bool IsConnected => _isConnected;
        public EndPoint RemoteEndPoint => _endpoint;
        public int ClientPort => _clientPort;
        public event Action<EndPoint> OnConnected;
        public event Action<EndPoint> OnConnecting;
        public event Action OnDisconnected;
        public event Action OpponentReadyEvent;
        public bool IsHost => _isHost;

        public bool OpponentReady
        {
            get => _opponentReady;
            set
            {
                if (_opponentReady == value) return;
                _opponentReady = value;
                OpponentReadyEvent?.Invoke();
            }
        }

        public NetworkState NetworkState
        {
            get => _networkState;
            private set
            {
                switch (value)
                {
                    case NetworkState.Connected:
                        OnConnected?.Invoke(_endpoint);
                        _networkState = NetworkState.Connected;
                        break;
                    case NetworkState.Connecting:
                        OnConnecting?.Invoke(_endpoint);
                        _networkState = NetworkState.Connecting;
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
        public ISynchronizationController SynchronizationController { get; set; }

        public void SetRemoteEndPointAndClientPort(bool isHost, string ip, int remotePort, int clientPort)
        {
            _ip = ip;
            _remoteEndpointPort = remotePort;
            _clientPort = clientPort;
            _isHost = isHost;
        }
        
        public async UniTask ConnectAsync(bool isHost, string ip, int remotePort, int clientPort, int timeOut = 5)
        {
            _ip = ip;
            _clientPort = clientPort;
            _remoteEndpointPort = remotePort;
            _isHost = isHost;
            await ConnectAsync(timeOut);
        }
        
        public async UniTask ConnectAsync(int timeout = 5)
        {
            const string path = "config.json";
            if(!File.Exists(path))
            {
                Debug.LogError("config.json is not found");
                NetworkConfig config = new ();
                config.defaultPort = NetworkProperties.DefaultPort;
                config.ipAddresses = new string[0];
                await File.WriteAllTextAsync(path, JsonUtility.ToJson(config));
                return;
            }
            var json = await File.ReadAllTextAsync(path);
            _networkConfig = JsonUtility.FromJson<NetworkConfig>(json);
            if (_gameManager.PlayerType == PlayerType.NotSelected)
            {
                Debug.LogError("PlayerType is not selected");
                _gameManager.PlayerType = PlayerType.Hakken;
            }
            _isHost = _gameManager.PlayerType == PlayerType.Hakken;
            _clientPort = NetworkProperties.DefaultPort;
            _remoteEndpointPort = NetworkProperties.DefaultPort;
            _isConnected = false;
            // すでに接続中か接続済みの場合はエラー
            if (NetworkState != NetworkState.Disconnected)
            {
                Debug.LogError("Already connected or connecting");
                return;
            }
            
            NetworkState = NetworkState.Connecting;
            _client = new UdpClient(_clientPort);
            if (_client == null)
            {
                Debug.LogError("Failed to create UdpClient");
                NetworkState = NetworkState.Disconnected;
                return;
            }
            
            // キャンセルトークン生成
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeout));
            var cancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutToken, _disposeCancellationTokenSource.Token).Token;

            // ACK を送信
            SearchOpponent(token);
            
            // ACK を受信
            var result = await ReceiveAckAsync(token);
            Debug.Log("Receive ACK");
            if (!result)
            {
                cancellationTokenSource.Cancel();
                Debug.LogError("Failed to connect");
                NetworkState = NetworkState.Disconnected;
                return;
            }
            
            // ちょっと待たないと相手が受信できない
            Debug.Log("Wait 1 sec");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            cancellationTokenSource.Cancel();
            _client.Connect(_ip, _remoteEndpointPort);
            NetworkState = NetworkState.Connected;
            Debug.Log("Connected");
            
            // 受信タスク開始
            ReceiveAsync(_disposeCancellationTokenSource.Token).Forget();
        }

        private async UniTask<bool> ReceiveAckAsync(CancellationToken token)
        {
            while(true)
            {
                Debug.Log($"Receive ACK isHost{_isHost}");
                var receiveTask = _client.ReceiveAsync();
                await UniTask.WaitUntil(() => receiveTask.IsCompleted || token.IsCancellationRequested);
                if (token.IsCancellationRequested)
                {
                    // タイムアウトなど
                    Debug.LogError("Receive Task is cancelled");
                    return false;
                }

                if (!receiveTask.IsCompleted)
                {
                    // 何かしらのエラー
                    Debug.LogError("Receive Task is not completed");
                    return false;
                }

                var result = receiveTask.Result;
                _endpoint = result.RemoteEndPoint;

                if (result.Buffer[0] == (_isHost ? 0xfe : 0xff))
                {
                    // 接続成功
                    _isConnected = true;
                    _ip = result.RemoteEndPoint.ToString();
                    return true;
                }
                else if (result.Buffer[0] == (_isHost ? 0xff : 0xfe))
                {
                    // 接続失敗
                    var randNum = result.Buffer[1] | (result.Buffer[2] << 8) | (result.Buffer[3] << 16) | (result.Buffer[4] << 24);
                    Debug.Log($"header:{result.Buffer[0]:X2} randNum:{randNum} _randNum:{_randNum}");
                    if (randNum == _randNum)
                    {
                        // 乱数が一致した場合は再送信
                        _randNum = Random.Range(int.MinValue, int.MaxValue);
                        Debug.Log("Same randNum");
                        continue;
                    }
                    if (randNum < _randNum)
                    {
                        // 自分の方が大きい場合はホスト
                        continue;
                    }
                    else
                    {
                        // 相手の方が大きい場合はクライアント
                        _isHost = false;
                        _gameManager.PlayerType = PlayerType.Kouken;
                        continue;
                    }
                }
            }
        }
        
        private async UniTask SendACKOnceAsync(string ip, int port)
        {
            var ack = (byte)(_isHost ? 0xff : 0xfe);
            var data = new byte[5];
            data[0] = ack;
            data[1] = (byte)_randNum;
            data[2] = (byte)(_randNum >> 8);
            data[3] = (byte)(_randNum >> 16);
            data[4] = (byte)(_randNum >> 24);
            await _client.SendAsync(data, data.Length, ip, port);
        }

        /// <summary>
        /// 対戦相手をネットワーク上から探す
        /// </summary>
        /// <param name="token"></param>
        private void SearchOpponent(CancellationToken token)
        {
            _randNum = Random.Range(int.MinValue, int.MinValue);
            Observable.Interval(TimeSpan.FromSeconds(0.2f), cancellationToken: token)
                .Subscribe(_ => SendAck2AllIpAddress());
        }

        /// <summary>
        /// 送れるアドレスに総当たりで ACK を 1 回送る
        /// </summary>
        /// <param name="token"></param>
        /// <param name="randNum"></param>
        private void SendAck2AllIpAddress()
        {
            if (_networkConfig.ipAddresses.Length == 0)
            {
                Debug.LogError("No IP Address");
                return;
            }
            foreach (var ip in _networkConfig.ipAddresses)
            {
                SendACKOnceAsync(ip, _networkConfig.defaultPort).Forget();
            }
        }
        
        private async UniTask ReceiveAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var receiveTask = _client.ReceiveAsync();
                await UniTask.WaitUntil(() => receiveTask.IsCompleted || token.IsCancellationRequested);
                if (token.IsCancellationRequested)
                {
                    receiveTask.Dispose();
                    return;
                }
                try
                {
                    if (receiveTask.Result.Buffer.Length <= 0) continue;
                }catch (Exception ex)
                {
                    return;
                }
                var reader = new BinaryReader(new MemoryStream(receiveTask.Result.Buffer));
                if (reader.ReadByte() != 0xaa)
                {
                    OpponentReady = false;
                    continue;
                }
                OpponentReady = true;
                if (SynchronizationController == null) continue;
                
                foreach(var synchronization in SynchronizationController.MonoSynchronizations)
                {
                    var length = reader.ReadInt32();
                    var data = reader.ReadBytes(length);
                    synchronization.FromBytes(data);
                }
            }
        }

        public void Initialize()
        {
            _isConnected = false;
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
            _client?.Dispose();
            _isConnected = false;
            NetworkState = NetworkState.Disconnected;
        }

        public void FixedTick()
        {
            if (!_isConnected || SynchronizationController == null) return;
            var writer = new BinaryWriter(new MemoryStream());
            // 0xaa はプレイ中
            writer.Write((byte) 0xaa);
            foreach (var synchronization in SynchronizationController.MonoSynchronizations)
            {
                var data = synchronization.ToBytes();
                writer.Write(data.Length);
                writer.Write(synchronization.ToBytes());
            }
            _client.Send(((MemoryStream)writer.BaseStream).ToArray(), (int)writer.BaseStream.Length);
        }
    }
}