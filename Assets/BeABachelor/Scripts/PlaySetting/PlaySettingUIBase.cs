using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.PlaySetting
{
    public abstract class PlaySettingUIBase : MonoBehaviour
    {
        [Inject] protected IGameManager _gameManager;
        [Inject] protected IPlaySettingManager _playSettingManager;
        [Inject] protected INetworkManager _networkManager;
        
        public abstract void Left();
        public abstract void Right();
        public abstract void Space();
        public abstract void Activate();
        public abstract void Deactivate();
    }
}