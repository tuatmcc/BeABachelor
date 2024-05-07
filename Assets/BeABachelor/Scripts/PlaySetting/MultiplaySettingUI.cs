using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.PlaySetting
{
    public class MultiplaySettingUI : PlaySettingUIBase
    {
        [SerializeField] private GameObject selector;
        [SerializeField] private InputField remoteIPInputField;
        [SerializeField] private InputField remotePortInputField;
        [SerializeField] private InputField localPortInputField;
        
        [Inject] private INetworkManager _networkManager;
        public override void Left()
        {
        }

        public override void Right()
        {
        }
        
        public override void Space()
        {
            _networkManager.SetRemoteEndPointAndClientPort(_gameManager.PlayerType == PlayerType.Hakken, 
                remoteIPInputField.text, 
                int.Parse(remotePortInputField.text), 
                int.Parse(localPortInputField.text));
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