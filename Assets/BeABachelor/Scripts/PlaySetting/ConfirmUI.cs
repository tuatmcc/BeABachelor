using System;
using System.Net;
using BeABachelor.Interface;
using BeABachelor.Networking;
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
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IPlaySettingManager _playSettingManager;
        
        private bool _failedFlag;

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
            _failedFlag = true;
        }
        
        private void HideAll()
        {
            connectingUI.SetActive(false);
            confirmUI.SetActive(false); 
            connectionFailedUI.SetActive(false);
        }
        
        private void Start()
        {
            _failedFlag = false;
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
            if (_networkManager.NetworkState != NetworkState.Connecting && _failedFlag)
            {
                HideAll();
                _failedFlag = false;
                _playSettingManager.State = PlaySettingState.PlayerType;
            }
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