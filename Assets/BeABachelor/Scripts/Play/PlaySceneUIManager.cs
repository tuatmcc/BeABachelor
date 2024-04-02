using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject waitOpponentPanel;
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IGameManager _gameManager;
        
        private void OnOpponentReady()
        {
            waitOpponentPanel.SetActive(false);
        }
        private void Start()
        {
            if (_gameManager.PlayType != PlayType.Multi) return;
            waitOpponentPanel.SetActive(true);
            _networkManager.OpponentReadyEvent += OnOpponentReady;
        }
    }
}