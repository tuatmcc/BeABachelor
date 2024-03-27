using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Networking.Play.Test;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace BeABachelor.Networking.Play
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string ip;
        [SerializeField] private int clientPort;
        [SerializeField] private int endpointPort;

        private UdpClient _client;
        private bool _isConnected;
        private List<byte[]> _receivedData;
        private EndPoint _endpoint;
        
        public bool IsConnected => _isConnected;

        private void Awake()
        {
            _client = new UdpClient(clientPort);
            _isConnected = false;
            _receivedData = new List<byte[]>();
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
        }
        
        public async UniTask ConnectAsync(int timeOut = 3)
        {
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
            _isConnected = false;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var receivedEndPoint = new IPEndPoint(IPAddress.Any, 0);
            UdpReceiveResult result;
            Observable.Interval(TimeSpan.FromSeconds(0.1f), cancellationToken: timeoutToken)
                .Subscribe(_ => _client.Send(new byte[] { 0xff }, 1, ip, endpointPort));

            do
            {
                var receiveTask = _client.ReceiveAsync();
                await UniTask.WaitUntil(() => receiveTask.IsCompleted || timeoutToken.IsCancellationRequested);
                _client.Close();
                if (timeoutToken.IsCancellationRequested)
                {
                    Debug.LogError("Connection timed out");
                    return;
                }
                result = receiveTask.Result;
            }while (result.Buffer[0] == 0xff && result.RemoteEndPoint.Equals(_endpoint) && !timeoutToken.IsCancellationRequested);
            
            _isConnected = true;
            _client.Connect(ip, endpointPort);
        }
    }
}