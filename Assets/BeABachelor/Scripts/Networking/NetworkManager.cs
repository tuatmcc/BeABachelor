﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Networking.Interface;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class NetworkManager : INetworkManager, IInitializable, IDisposable, IFixedTickable
    {
        private string _ip;
        private int _clientPort;
        private int _remoteEndpointPort;
        private UdpClient _client;
        private bool _isConnected;
        private EndPoint _endpoint;
        private CancellationTokenSource _disposeCancellationTokenSource;
        private bool _isHost;
        private NetworkState _networkState;

        public bool IsConnected => _isConnected;
        public EndPoint RemoteEndPoint => _endpoint;
        public int ClientPort => _clientPort;
        public event Action<EndPoint> OnConnected;
        public event Action<EndPoint> OnConnecting;
        public event Action OnDisconnected;
        public bool IsHost => _isHost;

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
        public SynchronizationController SynchronizationController { get; set; }

        public async UniTask ConnectAsync(bool isHost, string ip, int remotePort, int clientPort, int timeOut = 5)
        {
            NetworkState = NetworkState.Connecting;
            _ip = ip;
            _clientPort = clientPort;
            _remoteEndpointPort = remotePort;
            _endpoint = new IPEndPoint(IPAddress.Parse(_ip), _remoteEndpointPort);
            _client = new UdpClient(_clientPort);
            if (_client == null)
            {
                Debug.LogError("Failed to create UdpClient");
                NetworkState = NetworkState.Disconnected;
                return;
            }
            _isHost = isHost;
            _endpoint = new IPEndPoint(IPAddress.Parse(_ip), _remoteEndpointPort);
            _isConnected = false;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var cancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutToken, _disposeCancellationTokenSource.Token).Token;
            UdpReceiveResult result;
            var ack = (byte)(_isHost ? 0xff : 0xfe);
            var sendTask = Observable.Interval(TimeSpan.FromSeconds(0.2f), cancellationToken: token)
                .Subscribe(_ => _client.Send(new byte[] { ack }, 1, _ip, _remoteEndpointPort));

            var receiveTask = _client.ReceiveAsync();
            await UniTask.WaitUntil(() => receiveTask.IsCompleted || token.IsCancellationRequested);
            // ちょっと待たないと相手が受信できない
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            if (timeoutToken.IsCancellationRequested)
            {
                sendTask.Dispose();
                _client.Dispose();
                Debug.LogError("Connection timed out");
                NetworkState = NetworkState.Disconnected;
                return;
            }

            if (!receiveTask.IsCompleted)
            {
                Debug.LogError("Receive Task is not completed");
                NetworkState = NetworkState.Disconnected;
                return;
            }

            result = receiveTask.Result;
            if (result.Buffer[0] == (_isHost ? 0xfe : 0xff) && result.RemoteEndPoint.Equals(_endpoint) &&
                                                     !timeoutToken.IsCancellationRequested)
            {
                _isConnected = true;
                _client.Connect(_ip, _remoteEndpointPort);
                cancellationTokenSource.Cancel();
                Debug.Log("Connected");
                NetworkState = NetworkState.Connected;
                ReceiveAsync(_disposeCancellationTokenSource.Token).Forget();
                return;
            }
            
            Debug.LogError("Connection Failed");
            NetworkState = NetworkState.Disconnected;
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

                if (receiveTask.Result.Buffer.Length <= 0) continue;
                var reader = new BinaryReader(new MemoryStream(receiveTask.Result.Buffer));
                if (reader.ReadByte() != 0xaa) continue;
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