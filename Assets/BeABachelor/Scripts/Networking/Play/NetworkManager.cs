using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BeABachelor.Networking.Play.Test;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace BeABachelor.Networking.Play
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string _ip;
        [SerializeField] private int _port;
        [SerializeField] private int _endpointPort;
        [SerializeField] private NetworkTestGameManager _gameManager; 
        private UdpClient _udpClient;
        private Dictionary<int, TickData> _opponentTickData;
        private Dictionary<int, TickData> _playerTickData;
        private IPAddress _ipAddress;
        private IPEndPoint _endPoint;
        private Action<int> _tickProcess;
        private int _processedTick = 0;
        private CancellationTokenSource _cancellationTokenSource;
        private NetworkState _networkState = NetworkState.Preamble;
        
        public NetworkState GetNetworkState => _networkState;
        
        private void Awake()
        {
            _udpClient = new UdpClient(_port);
            _opponentTickData = new Dictionary<int, TickData>();
            _playerTickData = new Dictionary<int, TickData>();
            _ipAddress = IPAddress.Parse(_ip);
            _endPoint = new IPEndPoint(_ipAddress, _endpointPort);
            _tickProcess = _gameManager.IsHakken() ? HakkenTickProcess : KokenTickProcess;
            _cancellationTokenSource = new CancellationTokenSource();
            _networkState = NetworkState.Preamble;
            // flag + tickCount + playerPosition + enemyPosition + enableItemLength + enableItems
            _tickDataSize = 1 + 4 + 12 + 12 + 4 + _gameManager.GetEnableItems().Length;
            StartPreamble().Forget();
        }

        private void Update()
        {
            switch (_networkState)
            {
                case NetworkState.Preamble:
                    break;
                case NetworkState.Playing:
                    var tick = _gameManager.GetTickCount();
                    if (tick < _processedTick)
                    {
                        return;
                    }
            
                    var tickData = new TickData(tick, _gameManager.GetEnableItems(), _gameManager.GetPlayerPosition(), _gameManager.GetEnemyPosition());
                    _playerTickData[tick] = tickData;
                    SendData(tickData);
                    _processedTick++;
                    break;
                case NetworkState.Ended:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Disable()
        {
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
        }

        public void OnDestroy()
        {
            Disable();
        }

        private async UniTask StartPreamble()
        {
            var preambleTokenSource = new CancellationTokenSource();
            // Send preamble
            Debug.Log("Start Preamble");
            Observable.Interval(TimeSpan.FromSeconds(0.1), preambleTokenSource.Token).Subscribe(_ =>
            {
                _udpClient.Send(new []{(byte)1}, 1, _endPoint);
            });
            Debug.Log("Start Preambling");
            
            // Receive preamble
            UdpReceiveResult data;
            do
            {
                data = await _udpClient.ReceiveAsync();
            } while (data.Buffer.Length != 1 && data.Buffer[0] != 1);
            Debug.Log("Start");
            preambleTokenSource.Cancel();
            _networkState = NetworkState.Playing;
            _gameManager.GameStart();
            ReceiveDataAsync(_cancellationTokenSource.Token).Forget();
        }


        private void SendData(InteractionObject interactionObject)
        {
            if (interactionObject is TickData tickData)
            {
                using MemoryStream stream = new(_tickDataSize);
                stream.WriteByte(0);
                stream.Write(BitConverter.GetBytes(tickData.TickCount));
                stream.Write(BitConverter.GetBytes(tickData.PlayerPosition.x));
                stream.Write(BitConverter.GetBytes(tickData.PlayerPosition.y));
                stream.Write(BitConverter.GetBytes(tickData.PlayerPosition.z));
                stream.Write(BitConverter.GetBytes(tickData.EnemyPosition.x));
                stream.Write(BitConverter.GetBytes(tickData.EnemyPosition.y));
                stream.Write(BitConverter.GetBytes(tickData.EnemyPosition.z));
                stream.Write(BitConverter.GetBytes(tickData.EnableItems.Length));
                foreach (var enableItem in tickData.EnableItems)
                {
                    stream.WriteByte(enableItem ? (byte)1 : (byte)0);
                }

                var data = stream.ToArray();
                _udpClient.Send(data, data.Length, _endPoint);
            }
            else
            {
                var rerequest = (Rerequest)interactionObject;
                // flag + tickCount
                using MemoryStream stream = new(5);
                stream.WriteByte(1);
                stream.Write(BitConverter.GetBytes(rerequest.TickCount));
                var data = stream.ToArray();
                _udpClient.Send(data, data.Length, _endPoint);
            }
        }

        private async UniTask ReceiveDataAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var data = await _udpClient.ReceiveAsync();
                using BinaryReader reader = new(new MemoryStream(data.Buffer));
                var flag = reader.ReadByte();
                if (flag == 0)
                {
                    var tickCount = reader.ReadInt32();
                    var playerPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    var enemyPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    var enableItemLength = reader.ReadInt32();
                    bool[] enableItems = new bool[enableItemLength];
                    for (int i = 0; i < enableItemLength; i++)
                    {
                        enableItems[i] = reader.ReadByte() == 1;
                    }

                    var tickData = new TickData(tickCount, enableItems, enemyPosition, playerPosition);
                    _opponentTickData[tickCount] = tickData;
                    _tickProcess(tickCount);
                }
                else
                {
                    var tickCount = reader.ReadInt32();
                    SendData(_playerTickData[tickCount]);
                }
            }
        }

        private void HakkenTickProcess(int tick)
        {
            while(!_playerTickData.ContainsKey(tick)){}
            var tickData = _playerTickData[tick];
            var enemyTickData = _opponentTickData[tick];
            bool[] enableItems = _gameManager.GetEnableItems();
            for(int i = 0; i < tickData.EnableItems.Length; i++)
            {
                if (enableItems[i] && !tickData.EnableItems[i])
                {
                    _gameManager.SetEnableItems(i, false);
                    _gameManager.AddScore(100);
                }
                if (enableItems[i] && !enemyTickData.EnableItems[i])
                {
                    _gameManager.SetEnableItems(i, false);
                }
            }
            Vector3 enemyPosition = new (960.0f - enemyTickData.EnemyPosition.x, 1080.0f - enemyTickData.EnemyPosition.y, enemyTickData.EnemyPosition.z);
            _gameManager.SetEnemyPosition(enemyPosition);
        }
        
        private void KokenTickProcess(int tick)
        {
            var tickData = _playerTickData[tick];
            var enemyTickData = _opponentTickData[tick];
            bool[] enableItems = _gameManager.GetEnableItems();
            for(int i = 0; i < tickData.EnableItems.Length; i++)
            {
                if (enableItems[i] && !enemyTickData.EnableItems[i])
                {
                    _gameManager.SetEnableItems(i, false);
                }

                if (enableItems[i] && !tickData.EnableItems[i])
                {
                    _gameManager.SetEnableItems(i, false);
                    _gameManager.AddScore(100);
                }
            }
            
            Vector3 enemyPosition = new (960.0f - enemyTickData.EnemyPosition.x, 1080.0f - enemyTickData.EnemyPosition.y, enemyTickData.EnemyPosition.z);
            _gameManager.SetEnemyPosition(enemyPosition);
        }
    }
}