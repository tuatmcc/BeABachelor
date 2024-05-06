using System;
using System.Net;
using BeABachelor.Networking;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.PlaySetting
{
    public class ConnectingUI : PlaySettingUIBase
    {
        [SerializeField] private PlaySettingManager playSettingManager;
        
        [Inject] private INetworkManager _networkManager;
        public override void Left()
        {
            
        }

        public override void Right()
        {
            
        }

        public override void Space()
        {
            
        }

        public override void Back()
        {
            
        }

        public void OnConnect(EndPoint _)
        {
            playSettingManager.State = PlaySettingState.Confirm;
        }
        
        public void OnDisconnect()
        {
            playSettingManager.State = PlaySettingState.ConnectionFailed;
        }
        
        public override void Activate()
        {
            gameObject.SetActive(true);
            _networkManager.OnConnected += OnConnect;
            _networkManager.OnDisconnected += OnDisconnect;
            _networkManager.ConnectAsync();
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
            _networkManager.OnConnected -= OnConnect;
            _networkManager.OnDisconnected -= OnDisconnect;
        }
    }
}