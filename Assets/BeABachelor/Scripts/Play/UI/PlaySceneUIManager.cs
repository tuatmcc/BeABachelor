using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class PlaySceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject waitOpponentPanel;
        [SerializeField] private CountDownText countDownText;
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;
        
        private void OnOpponentReady()
        {
            waitOpponentPanel.SetActive(false);
        }
        private void Start()
        {
            _playSceneManager.OnCountChanged += countDownText.CountText;
            if (_gameManager.PlayType != PlayType.Multi) return;
            if(!_networkManager.OpponentReady) waitOpponentPanel?.SetActive(true);
            _networkManager.OpponentReadyEvent += OnOpponentReady;
        }
        private void OnDestroy()
        {
            _playSceneManager.OnCountChanged -= countDownText.CountText;
            if (_gameManager.PlayType != PlayType.Multi) return;
            _networkManager.OpponentReadyEvent -= OnOpponentReady;
        }
    }
}