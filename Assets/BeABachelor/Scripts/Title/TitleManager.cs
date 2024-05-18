using System;
using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.Title
{
    
    [RequireComponent(typeof(PlayerInput))]
    public class TitleManager : MonoBehaviour, ITitleManager
    {
        public Action PlayFadeIn { get; set; }
        public Action PlayFadeOut { get; set; }
        
        [Inject] private IGameManager _gameManager;
        [Inject] private IAudioManager _audioManager;
        private PlayerInput _playerInput;
        private bool _sceneChangeFlag;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _sceneChangeFlag = false;
        }

        private void Start()
        {
            PlayFadeIn?.Invoke();
        }

        private void OnKeyDown(InputAction.CallbackContext context)
        {
            if (context.action.name == "Space" && !_sceneChangeFlag)
            {
                _sceneChangeFlag = true;
                ((TitleAudioManager)_audioManager).PlayConfirmSE();
                StateChangeWaitFade().Forget();
                Debug.Log("Space");
            }
        }

        private async UniTask StateChangeWaitFade()
        {
            PlayFadeOut?.Invoke();
            await UniTask.Delay(500);
            _gameManager.GameState = GameState.Setting;
        }
        
        private void OnEnable()
        {
            _playerInput.actions["Space"].performed += OnKeyDown;
        }
        
        private void OnDisable()
        {
            _playerInput.actions["Space"].performed -= OnKeyDown;
        }
    }
}