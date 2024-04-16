using System;
using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.PlaySetting
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlaySettingManager : MonoBehaviour, IPlaySettingManager
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private PlaySettingUIBase playModeTypeUI;
        [SerializeField] private PlaySettingUIBase playerTypeUI;
        [SerializeField] private PlaySettingUIBase multiplaySettingUI;
        [SerializeField] private PlaySettingUIBase confirmUI;
        
        [SerializeField] private GameObject hakken;
        [SerializeField] private GameObject kouken;
        
        [Inject] private IGameManager _gameManager;
        private PlaySettingState _state;
        private PlaySettingUIBase _activeUI;
        private bool _sceneChangeFlag;

        public Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        public Action PlayFadeIn { get; set; }
        public Action PlayFadeOut { get; set; }
        public PlaySettingState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPlaySettingStateChanged?.Invoke(_state);
            }
        }

        public PlayerType PlayerType { get; set; }

        public PlayType PlayType { get; set; }

        public void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            _gameManager.PlayerType = PlayerType.Kouken;
            _sceneChangeFlag = false;
        }

        public void OnEnable()
        {
            playerInput.actions["Left"].performed += OnSelect;
            playerInput.actions["Right"].performed += OnSelect;
            playerInput.actions["Space"].performed += OnSelect;
            playerInput.actions["Back"].performed += OnSelect;
        }

        public void OnDisable()
        {
            playerInput.actions["Left"].performed -= OnSelect;
            playerInput.actions["Right"].performed -= OnSelect;
            playerInput.actions["Space"].performed -= OnSelect;
            playerInput.actions["Back"].performed -= OnSelect;
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case "Left":
                    _activeUI.Left();
                    break;
                case "Right":
                    _activeUI.Right();
                    break;
                case "Space":
                    _activeUI.Space();
                    NextState();
                    break;
                case "Back":
                    BackState();
                    break;
            }
        }

        private void NextState()
        {
            switch (_state)
            {
                case PlaySettingState.PlayMode:
                    State = PlaySettingState.PlayerType;
                    break;
                case PlaySettingState.PlayerType:
                    State = _gameManager.PlayType == PlayType.Solo ? PlaySettingState.Confirm : PlaySettingState.MultiplaySetting;
                    break;
                case PlaySettingState.MultiplaySetting:
                    State = PlaySettingState.Confirm;
                    break;
                case PlaySettingState.Confirm:
                    StateChangeWaitFade().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private async UniTask StateChangeWaitFade()
        {
            if (_sceneChangeFlag) return;
            _sceneChangeFlag = true;
            await UniTask.Delay(1000);
            PlayFadeOut?.Invoke();
            await UniTask.Delay(1500);
            _gameManager.GameState = GameState.Ready;
        }
        
        private void BackState()
        {
            switch (_state)
            {
                case PlaySettingState.PlayMode:
                    break;
                case PlaySettingState.PlayerType:
                    State = PlaySettingState.PlayMode;
                    break;
                case PlaySettingState.MultiplaySetting:
                    State = PlaySettingState.PlayerType;
                    break;
                case PlaySettingState.Confirm:
                    State = _gameManager.PlayType == PlayType.Solo ? PlaySettingState.PlayerType : PlaySettingState.MultiplaySetting;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnPlaySettingStateChange(PlaySettingState state)
        {
            switch (state)
            {
                case PlaySettingState.PlayMode:
                    playModeTypeUI.Activate();
                    playerTypeUI.Deactivate();
                    multiplaySettingUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = playModeTypeUI;
                    break;
                case PlaySettingState.PlayerType:
                    playModeTypeUI.Deactivate();
                    playerTypeUI.Activate();
                    multiplaySettingUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = playerTypeUI;
                    break;
                case PlaySettingState.MultiplaySetting:
                    playModeTypeUI.Deactivate();
                    playerTypeUI.Deactivate();
                    multiplaySettingUI.Activate();
                    confirmUI.Deactivate();
                    _activeUI = multiplaySettingUI;
                    break;
                case PlaySettingState.Confirm:
                    playModeTypeUI.Deactivate();
                    playerTypeUI.Deactivate();
                    multiplaySettingUI.Deactivate();
                    confirmUI.Activate();
                    _activeUI = confirmUI;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void Start()
        {
            OnPlaySettingStateChanged += OnPlaySettingStateChange;
            State = PlaySettingState.PlayMode;
        }

        private void OnDestroy()
        {
            OnPlaySettingStateChanged -= OnPlaySettingStateChange;
        }
    }
}