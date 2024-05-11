using System.Net;

namespace BeABachelor.PlaySetting
{
    public class ConnectingUI : PlaySettingUIBase
    {
        public void OnEnable()
        {
            _networkManager.OnConnected += OnConnected;
            _networkManager.OnDisconnected += OnDisconnected;
        }
        
        public void OnDisable()
        {
            _networkManager.OnConnected -= OnConnected;
            _networkManager.OnDisconnected -= OnDisconnected;
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
        
        private void OnDisconnected()
        {
            _playSettingManager.State = PlaySettingState.Failed;
        }

        private void OnConnected(EndPoint _)
        {
            _playSettingManager.State = PlaySettingState.Confirm;
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