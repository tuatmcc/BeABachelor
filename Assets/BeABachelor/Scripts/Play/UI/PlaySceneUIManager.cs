using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using System;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class PlaySceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject waitOpponentPanel;
        [SerializeField] private CountDownText countDownText;
        [SerializeField] private GameObject finishImage;
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;
        
        private void OnOpponentReady()
        {
            try
            {
                waitOpponentPanel.SetActive(false);
            } catch(Exception e)
            {
                return; 
            }
        }
        
        private void EnableFinishImage(GameState state)
        {
            if(state == GameState.Finished)
                finishImage.SetActive(true);

        }
        
        private void Start()
        {
            _playSceneManager.OnCountChanged += countDownText.CountText;
            _gameManager.OnGameStateChanged += EnableFinishImage;
            if (_gameManager.PlayType != PlayType.Multi) return;
            
            // これ以降はマルチプレイのみ
            
            if(!_networkManager.OpponentReady) waitOpponentPanel?.SetActive(true);
            _networkManager.OpponentReadyEvent += OnOpponentReady;
        }
        private void OnDestroy()
        {
            _gameManager.OnGameStateChanged -= EnableFinishImage;
            _playSceneManager.OnCountChanged -= countDownText.CountText;
            if (_gameManager.PlayType != PlayType.Multi) return;
            _networkManager.OpponentReadyEvent -= OnOpponentReady;
        }
    }
}