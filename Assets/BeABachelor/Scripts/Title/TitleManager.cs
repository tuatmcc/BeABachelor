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
                StateChangeWaitFade().Forget();
                Debug.Log("Space");
            }
        }

        private async UniTask StateChangeWaitFade()
        {
            await UniTask.Delay(1000);
            PlayFadeOut?.Invoke();
            await UniTask.Delay(1500);
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