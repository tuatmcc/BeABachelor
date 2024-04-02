using System;
using System.Net;
using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.PlaySetting
{
    public class ConfirmUI : PlaySettingUIBase
    {
        [SerializeField] private GameObject connectingUI;
        [SerializeField] private GameObject confirmUI;
        [SerializeField] private GameObject connectionFailedUI;
        
        [Inject] private IGameManager _gameManager;
        [Inject] private INetworkManager _networkManager;

        private void OnConnect(EndPoint _)
        {
                connectingUI.SetActive(false);
                confirmUI.SetActive(true);
                connectionFailedUI.SetActive(false);
        }
        
        private void OnDisconnect()
        {
            connectingUI.SetActive(false);
            confirmUI.SetActive(false);
            connectionFailedUI.SetActive(true);
        }
        
        private void Start()
        {
            _networkManager.OnConnected += OnConnect;
            _networkManager.OnDisconnected += OnDisconnect;
        }

        private void OnDestroy()
        {
            _networkManager.OnConnected -= OnConnect;
            _networkManager.OnDisconnected -= OnDisconnect;
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override void Space()
        {
        }

        public override void Activate()
        {
            gameObject.SetActive(true);
            if (_gameManager.PlayType == PlayType.Multi)
            {
                _networkManager.ConnectAsync().Forget();
                connectingUI.SetActive(true);
                confirmUI.SetActive(false);
                connectionFailedUI.SetActive(false);
            }
            else
            {
                confirmUI.SetActive(true);
                connectingUI.SetActive(false);
                connectionFailedUI.SetActive(false);
            }
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
            _networkManager.Disconnect();
        }
    }
}