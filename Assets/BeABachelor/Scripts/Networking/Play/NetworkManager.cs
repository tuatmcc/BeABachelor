﻿using System;
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
            _isConnected = false;
            _receivedData = new List<byte[]>();
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
        }

        private void OnDestroy()
        {
            _client?.Dispose();
        }

        public async UniTask ConnectAsync(int timeOut = 5)
        {
            _client = new UdpClient(clientPort);
            if (_client == null)
            {
                Debug.LogError("Failed to create UdpClient");
                return;
            }
            _endpoint = new IPEndPoint(IPAddress.Parse(ip), endpointPort);
            _isConnected = false;
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(timeOut));
            var cancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutToken, this.GetCancellationTokenOnDestroy()).Token;
            UdpReceiveResult result;
            var sendTask = Observable.Interval(TimeSpan.FromSeconds(0.2f), cancellationToken: token)
                .Subscribe(_ =>
                {
                    Debug.Log("Send");
                    _client.Send(new byte[] { 0xff }, 1, ip, endpointPort);
                });

            // _client.ReceiveAsync();
            Debug.Log("Connecting");
            var receiveTask = _client.ReceiveAsync();
            await UniTask.WaitUntil(() => receiveTask.IsCompleted || token.IsCancellationRequested);
            Debug.Log("Receive End");
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
                _client.Connect(ip, endpointPort);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                cancellationTokenSource.Cancel();
                Debug.Log("Connected");
                return;
            }
            
            Debug.Log("Connection Failed");
        }
    }
}