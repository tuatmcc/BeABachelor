using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Networking.Interface;
using BeABachelor.Networking.Play.Test;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BeABachelor.Networking.Play
{
    public class NetworkManager : INetworkManager, IInitializable, IDisposable
    {
        private string _ip;
        private int _clientPort;
        private int _remoteEndpointPort;
        private UdpClient _client;
        private bool _isConnected;
        private Queue<byte[]> _receivedData;
        private EndPoint _endpoint;
        private CancellationTokenSource _disposeCancellationTokenSource;
        private UniTask _receiveTask;

        public bool IsConnected => _isConnected;
        public EndPoint RemoteEndPoint => _endpoint;
        public int ClientPort => _clientPort;
        public event Action<EndPoint> OnConnected;
        public event Action OnDisconnected;
        public event Action<byte[]> OnReceived;
        
        public async UniTask ConnectAsync(int timeOut = 5)
        {
            _client = new UdpClient(_clientPort);
            if (_client == null)
            {
                Debug.LogError("Failed to create UdpClient");
                return;
            }
            _endpoint = new IPEndPoint(IPAddress.Parse(_ip), _remoteEndpointPort);
            _isConnected = false;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var cancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutToken, _disposeCancellationTokenSource.Token).Token;
            UdpReceiveResult result;
            var sendTask = Observable.Interval(TimeSpan.FromSeconds(0.2f), cancellationToken: token)
                .Subscribe(_ => _client.Send(new byte[] { 0xff }, 1, _ip, _remoteEndpointPort));

            var receiveTask = _client.ReceiveAsync();
            await UniTask.WaitUntil(() => receiveTask.IsCompleted || token.IsCancellationRequested);
            // ちょっと待たないと相手が受信できない
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            if (timeoutToken.IsCancellationRequested)
            {
                sendTask.Dispose();
                _client.Dispose();
                Debug.LogError("Connection timed out");
                return;
            }

            if (!receiveTask.IsCompleted)
            {
                Debug.LogError("Receive Task is not completed");
                return;
            }

            result = receiveTask.Result;
            if (result.Buffer[0] == 0xff && result.RemoteEndPoint.Equals(_endpoint) &&
                !timeoutToken.IsCancellationRequested)
            {
                _isConnected = true;
                _client.Connect(_ip, _remoteEndpointPort);
                cancellationTokenSource.Cancel();
                Debug.Log("Connected");
                OnConnected?.Invoke(_endpoint);
                _receiveTask = Observable.F;
                return;
            }
            
            Debug.LogError("Connection Failed");
        }
        
        public async UniTask SendAsync(IBinariable binariable)
        {
            if (!_isConnected)
            {
                Debug.LogError("Not connected");
                return;
            }
            var bytes = binariable.ToBytes();
            await _client.SendAsync(bytes, bytes.Length);
        }

        public void Initialize()
        {
            _isConnected = false;
            _receivedData = new Queue<byte[]>();
            _endpoint = new IPEndPoint(IPAddress.Parse(_ip), _remoteEndpointPort);
            _disposeCancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _disposeCancellationTokenSource?.Cancel();
        }
        
        public void Disconnect()
        {
            _client?.Dispose();
            _isConnected = false;
            OnDisconnected?.Invoke();
        }
    }
}