using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking.Play.Test
{
    public class NetworkBebug : MonoBehaviour
    {
        [SerializeField] private string ipAddress;
        [SerializeField] private int clientPort;
        [SerializeField] private int remotePort;
        [SerializeField] private PlayerType playerType;
        [SerializeField] private NetworkState networkState = NetworkState.Disconnected;
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IGameManager _gameManager;
        
        [ContextMenu("Change Scene")]
        public void ChangeScene()
        {
            _networkManager.OnConnected += _ =>
            {
                networkState = NetworkState.Connected;
                _gameManager.GameState = GameState.Ready;
            };
            _networkManager.OnConnecting += _ =>
            {
                networkState = NetworkState.Connecting;
            };
            _gameManager.PlayerType = playerType;
            _networkManager.ConnectAsync(playerType == PlayerType.Hakken, ipAddress, remotePort, clientPort).Forget();
        }
    }
}