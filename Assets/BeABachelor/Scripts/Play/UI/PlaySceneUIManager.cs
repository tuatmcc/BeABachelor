using BeABachelor.Interface;
using BeABachelor.Networking.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class PlaySceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject waitOpponentPanel;
        [SerializeField] private CountDownText countDownText;
        [SerializeField] private GameObject finishImage;
        [SerializeField] private GameObject atsumeroPanel;
        [SerializeField] private Animator atsumeroPanelAnimator;
        
        [Inject] private INetworkManager _networkManager;
        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;
        
        private PlayerInput _playerInput;
        private InputActionAsset _move;
        private int _isMovingHash = Animator.StringToHash("isMoving");
        private int _isStartHash = Animator.StringToHash("startend");
        
        private void OnOpponentReady(GameState state)
        {
            if (state == GameState.CountDown)
                waitOpponentPanel.SetActive(false);
        }
        
        private void EnableFinishImage(GameState state)
        {
            if(state == GameState.Finished)
                finishImage.SetActive(true);

        }
        
        private void OnMoveTrue(InputAction.CallbackContext _)
        {
            atsumeroPanelAnimator.SetBool(_isMovingHash, true);
        }
        
        private void OnMoveFalse(InputAction.CallbackContext _)
        {
            atsumeroPanelAnimator.SetBool(_isMovingHash, false);
        }
        
        private void OnGameStatePlayingFinish(GameState state)
        {
            if (state is GameState.Playing or GameState.Finished)
                atsumeroPanelAnimator.SetTrigger(_isStartHash);
        }
        
        private void Start()
        {
            _playSceneManager.OnCountChanged += countDownText.CountText;
            _gameManager.OnGameStateChanged += EnableFinishImage;
            _gameManager.OnGameStateChanged += OnGameStatePlayingFinish;
            _playerInput = GetComponent<PlayerInput>();
            _move = _playerInput.actions;
            _move["Move"].performed += OnMoveTrue;
            _move["Move"].canceled += OnMoveFalse;
            
            if (_gameManager.PlayType == PlayType.Solo) return;
            
            // これ以降はマルチプレイのみ
            
            if(!_networkManager.OpponentReady) waitOpponentPanel?.SetActive(true);
            _gameManager.OnGameStateChanged += OnOpponentReady;
        }
        private void OnDestroy()
        {
            _gameManager.OnGameStateChanged -= EnableFinishImage;
            _playSceneManager.OnCountChanged -= countDownText.CountText;
            _gameManager.OnGameStateChanged -= OnGameStatePlayingFinish;
            _move["Move"].performed -= OnMoveTrue;
            _move["Move"].canceled -= OnMoveFalse;
            if (_gameManager.PlayType == PlayType.Solo) return;
            _gameManager.OnGameStateChanged -= OnOpponentReady;
        }
    }
}