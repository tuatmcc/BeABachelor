using System;
using System.Net;

namespace BeABachelor.PlaySetting
{
    public class SearchingUI : PlaySettingUIBase
    {
        private void Start()
        {
            _networkManager.OnConnecting += OnConnecting;
            _networkManager.OnDisconnected += OnDisconnect;
        }
        
        private void OnDisable()
        {
            _networkManager.OnConnecting -= OnConnecting;
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

        private void OnConnecting()
        {
            _playSettingManager.State = PlaySettingState.Connecting;
        }

        private void OnDisconnect()
        {
            _playSettingManager.State = PlaySettingState.Failed;
        }
        
        public override void Activate()
        {
            gameObject.SetActive(true);
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}