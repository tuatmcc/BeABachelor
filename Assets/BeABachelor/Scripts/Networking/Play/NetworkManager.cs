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

        private void Start()
        {
            _client = new UdpClient(clientPort);
            _isConnected = false;
            _receivedData = new List<byte[]>();
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
        }
        
        public async UniTask ConnectAsync(int timeOut = 10)
        {
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
            _isConnected = false;
            var connectedTokenSource = new CancellationTokenSource();
            var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeOut)).Token;
            var token = CancellationTokenSource.CreateLinkedTokenSource(timeoutToken, connectedTokenSource.Token).Token;
            var receivedEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Observable.Interval(TimeSpan.FromSeconds(0.1f), cancellationToken: token)
                .Subscribe(_ => _client.Send(new byte[] { 0xff }, 1, ip, endpointPort));
            await UniTask.WaitUntil(() => _client.Receive(ref receivedEndPoint)[0] == 0xff && receivedEndPoint.Equals(_endpoint), cancellationToken: timeoutToken);
            
            if(timeoutToken.IsCancellationRequested)
            {
                Debug.LogError("Connection timed out");
                return;
            }
            
            connectedTokenSource.Cancel();
            _isConnected = true;
            _client.Connect(ip, endpointPort);
        }
    }
}