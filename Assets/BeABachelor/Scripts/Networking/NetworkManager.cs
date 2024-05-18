using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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
        private IPAddress _selfIPAddress;
        private CancellationTokenSource _disposeCancellationTokenSource;
        private CancellationTokenSource _sendTickCancellationTokenSource;
        private bool _isHost;
        private bool _opponentReady;
        private NetworkState _networkState;
        private float _packetSendInterval;
        private int _IPIndex;

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
                if (!(_opponentReady ^ value)) return;
                _opponentReady = value;
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
                OnNetworkStateChanged?.Invoke(value);
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
            OpponentReady = false;
            NetworkState = NetworkState.Searching;

            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var broadcastCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(broadcastCancellationTokenSource.Token,
                timeoutToken, _disposeCancellationTokenSource.Token).Token;

            var networkConfig = JsonConfigure.NetworkConfig;
            _clientPort = networkConfig.port;
            _IPIndex = networkConfig.IPv4Index;
            _client = new UdpClient(_clientPort);
            _client.EnableBroadcast = true;

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

                try
                {
                    // 受信待ち
                    await UniTask.WaitUntil(() => broadcastReceiveTask.IsCompleted, cancellationToken: token);
                }
                catch (Exception e)
                {
                    // 強制タイムアウト
                    Debug.LogError("Connection timed out");
                    _client.Close();
                    // broadcastReceiveTask.Dispose();
                    broadcastCancellationTokenSource.Cancel(); // ブロードキャストを止める
                    NetworkState = NetworkState.Disconnected;
                    return;
                }

                // 受信成功
                if (NetworkState == NetworkState.Disconnected) return;
                result = broadcastReceiveTask.Result;

                // 有効なデータでないときは再受信する
            } while (!ValidAck(result));

            // ちょっと待たないと相手が受信できない
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _disposeCancellationTokenSource.Token);
            broadcastCancellationTokenSource.Cancel(); // ブロードキャストを止める

            _endpoint = result.RemoteEndPoint;
            Debug.Log($"Start connection with {_endpoint}");
            _client.Connect((IPEndPoint)_endpoint);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _disposeCancellationTokenSource.Token);
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
            _packetSendInterval = 1f / JsonConfigure.NetworkConfig.packetSendRate;
            _sendTickCancellationTokenSource = new CancellationTokenSource();
            StartSendTick(CancellationTokenSource.CreateLinkedTokenSource(_sendTickCancellationTokenSource.Token,
                _disposeCancellationTokenSource.Token).Token);

            ReceiveTask().Forget();
        }

        private void SearchPlayer(CancellationToken token)
        {
            _selfIPAddress = null;
            var hostName = "";
            var ip = Dns.GetHostEntry(hostName);
            var number = _IPIndex;
            IPAddress directedBroadcastAddress = null;
            foreach (var address in ip.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (number > 0)
                    {
                        number--;
                        continue;
                    }

                    _selfIPAddress = address;
                    break;
                }
            }

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var info in interfaces)
            {
                var target = info.GetIPProperties().UnicastAddresses
                    .Where(address => address.Address.AddressFamily == AddressFamily.InterNetwork);
                if (!target.Any()) continue;
                if (!target.First().Address.Equals(_selfIPAddress)) continue;
                var address_bytes = target.First().Address.GetAddressBytes();
                var mask_bites = target.First().IPv4Mask.GetAddressBytes();
                var directedBroadcastAddressBit =
                    BitConverter.ToUInt32(address_bytes) | ~BitConverter.ToUInt32(mask_bites);
                directedBroadcastAddress = new IPAddress(directedBroadcastAddressBit);
            }

            Debug.Log($"This IP is {_selfIPAddress}");
            Debug.Log($"Start broadcast to {directedBroadcastAddress}:{_clientPort}");
            Observable.Interval(TimeSpan.FromSeconds(0.1f), token)
                .Subscribe(_ =>
                    _client.Send(
                        _selfIPAddress.GetAddressBytes(),
                        _selfIPAddress.GetAddressBytes().Length,
                        new IPEndPoint(directedBroadcastAddress, _clientPort))
                );
        }

        private bool ValidAck(UdpReceiveResult result)
        {
            // Received length == IPv4 length and the remote endpoint address is not the same as _selfIPAddress
            return result.Buffer.Length == 4 && !result.RemoteEndPoint.Address.Equals(_selfIPAddress);
        }

        private bool IsWaitNegotiation(IPEndPoint endPoint)
        {
            var myIp = BitConverter.ToUInt32(
                ((IPEndPoint)_client.Client.LocalEndPoint).Address.GetAddressBytes().Reverse().ToArray(), 0);
            var opponentIp = BitConverter.ToUInt32(endPoint.Address.GetAddressBytes().Reverse().ToArray(), 0);
            Debug.Log(
                $"IsWaitNegotiation My IP: {myIp}, Opponent IP: {opponentIp} {((IPEndPoint)_client.Client.LocalEndPoint).Address}");
            return opponentIp < myIp;
        }

        private async UniTask<bool> SendNegotiationAsync(int timeOut)
        {
            var random = Random.Range(0, 2);
            for (int i = 0; i < 10; i++)
            {
                if (NetworkState == NetworkState.Disconnected) return false;
                await _client.SendAsync(new byte[] { 0x02, (byte)random }, 2);
                await UniTask.WaitForSeconds(0.1f);
            }

            _gameManager.PlayerType = random == 0 ? PlayerType.Kouken : PlayerType.Hakken;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut + 3));
            var negotiationCancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(negotiationCancellationTokenSource.Token,
                timeoutToken, _disposeCancellationTokenSource.Token).Token;
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
            } while (!ValidNegotiation(result.Buffer));

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
                try
                {
                    await UniTask.WaitUntil(() => negotiationReceiveTask.IsCompleted, cancellationToken: token);
                }
                catch (Exception e)
                {
                    // タイムアウト
                    Debug.LogError("Connection timed out");
                    _client.Close();
                    negotiationReceiveTask.Dispose();
                    negotiationCancellationTokenSource.Cancel();
                    return false;
                }

                if (NetworkState == NetworkState.Disconnected) return false;
                result = negotiationReceiveTask.Result;
            } while (!ValidNegotiation(result.Buffer));

            negotiationCancellationTokenSource.Cancel();
            _gameManager.PlayerType = result.Buffer[1] == 0 ? PlayerType.Hakken : PlayerType.Kouken;
            for (int i = 0; i < 10; i++)
            {
                if (NetworkState == NetworkState.Disconnected) return false;
                await _client.SendAsync(new byte[] { 0x02, result.Buffer[1] }, 2);
                await UniTask.WaitForSeconds(0.1f, cancellationToken: _disposeCancellationTokenSource.Token);
            }

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

                if (NetworkState == NetworkState.Disconnected) return;
                ReflectReceivedData(task.Result.Buffer).Forget();
            }
        }

        private UniTask ReflectReceivedData(byte[] receiveData)
        {
            var reader = new BinaryReader(new MemoryStream(receiveData));
            if (reader.ReadByte() != 0xaa) return UniTask.CompletedTask;
            if (SynchronizationController == null) return UniTask.CompletedTask;
            // これ以降 PlayScene での処理
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

            return UniTask.CompletedTask;
        }

        public void Initialize()
        {
            _opponentReady = false;
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
                _client?.Close();
                NetworkState = NetworkState.Disconnected;
                OpponentReady = false;
                _sendTickCancellationTokenSource?.Cancel();
                return UniTask.CompletedTask;
            }).Forget();
        }

        public void FixedTick()
        {
        }

        private void StartSendTick(CancellationToken token)
        {
            UniTask.Create(async () =>
            {
                while (IsConnected && !token.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_packetSendInterval), cancellationToken: token);
                    if (!IsConnected || SynchronizationController == null) continue;
                    try
                    {
                        var writer = new BinaryWriter(new MemoryStream());
                        // 0xaa はプレイ中
                        writer.Write((byte)0xaa);
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
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            });
        }
    }
}