using System;
using BeABachelor.Interface;
using BeABachelor.Networking;
using BeABachelor.Networking.Interface;
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
        [SerializeField] private PlaySettingUIBase connectingUI;
        [SerializeField] private PlaySettingUIBase conectFailedUI;
        [SerializeField] private PlaySettingUIBase confirmUI;
        
        [Inject] private IGameManager _gameManager;
        [Inject] private INetworkManager _networkManager;
        private PlaySettingState _state;
        private PlaySettingUIBase _activeUI;
        private bool _sceneChangeFlag;
        private bool _ignoreBack;

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
                if (_state != PlaySettingState.Confirm)
                {
                    _ignoreBack = false;
                }
                else if(_state == PlaySettingState.Connecting)
                {
                    _ignoreBack = true;
                }
            }
        }

        public PlayerType PlayerType { get; set; }

        public PlayType PlayType { get; set; }

        public void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            _gameManager.PlayerType = PlayerType.Kouken;
            _sceneChangeFlag = false;
            _ignoreBack = false;
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
                    break;
                case "Back":
                    _activeUI.Back();
                    break;
            }
        }

        public void NextState()
        {
            switch (_state)
            {
                case PlaySettingState.PlayMode:
                    State = _gameManager.PlayType == PlayType.Solo ? PlaySettingState.Confirm : PlaySettingState.Connecting;
                    break;
                case PlaySettingState.Connecting:
                    switch (_networkManager.NetworkState)
                    {
                        case NetworkState.Connected:
                            State = PlaySettingState.Confirm;
                            break;
                        case NetworkState.Disconnected:
                            State = PlaySettingState.ConnectionFailed;
                            break;
                        case NetworkState.Connecting:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PlaySettingState.ConnectionFailed:
                    State = PlaySettingState.PlayMode;
                    break;
                case PlaySettingState.Confirm:
                    ReadyStateChangeWaitFadeAsync().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask ShowConnectWaitUI()
        {
            await UniTask.WaitWhile(() => _networkManager.NetworkState == NetworkState.Connecting);
            if (_networkManager.NetworkState == NetworkState.Connected)
            {
                State = PlaySettingState.Confirm;
            }
        }

        public void ReadyStateChangeWaitFade()
        {
            ReadyStateChangeWaitFadeAsync().Forget();
        }
        
        private async UniTask ReadyStateChangeWaitFadeAsync()
        {
            if (_sceneChangeFlag) return;
            _sceneChangeFlag = true;

            if (_gameManager.PlayType == PlayType.Multi)
            {
                if (!_networkManager.IsConnected)
                {
                    _sceneChangeFlag = false;
                    return;
                }
            }

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
                    _gameManager.GameState = GameState.Title;
                    break;
                case PlaySettingState.Connecting:
                    break;
                case PlaySettingState.ConnectionFailed:
                    State = PlaySettingState.PlayMode;
                    break;
                case PlaySettingState.Confirm:
                    if (_gameManager.PlayType == PlayType.Solo)
                        State = PlaySettingState.PlayMode;
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
                    connectingUI.Deactivate();
                    conectFailedUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = playModeTypeUI;
                    break;
                case PlaySettingState.Connecting:
                    playModeTypeUI.Deactivate();
                    connectingUI.Activate();
                    conectFailedUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = connectingUI;
                    break;
                case PlaySettingState.ConnectionFailed:
                    playModeTypeUI.Deactivate();
                    connectingUI.Deactivate();
                    conectFailedUI.Activate();
                    confirmUI.Deactivate();
                    _activeUI = conectFailedUI;
                    break;
                case PlaySettingState.Confirm:
                    playModeTypeUI.Deactivate();
                    connectingUI.Deactivate();
                    conectFailedUI.Deactivate();
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