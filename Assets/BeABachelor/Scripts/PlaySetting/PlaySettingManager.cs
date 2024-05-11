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
        [SerializeField] private PlaySettingUIBase searchUI;
        [SerializeField] private PlaySettingUIBase connectingUI;
        [SerializeField] private PlaySettingUIBase connectFailedUI;
        [SerializeField] private PlaySettingUIBase confirmUI;
        
        [SerializeField] private GameObject hakken;
        [SerializeField] private GameObject kouken;
        
        [Inject] private IGameManager _gameManager;
        [Inject] private INetworkManager _networkManager;
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
                    break;
                case "Back":
                    
                    break;
            }
        }
        public async UniTask StateChangeWaitFade()
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

        private void OnPlaySettingStateChange(PlaySettingState state)
        {
            Debug.Log($"OnPlaySettingStateChange: {state}");
            switch (state)
            {
                case PlaySettingState.PlayMode:
                    playModeTypeUI.Activate();
                    searchUI.Deactivate();
                    connectingUI.Deactivate();
                    connectFailedUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = playModeTypeUI;
                    break;
                case PlaySettingState.Searching:
                    playModeTypeUI.Deactivate();
                    searchUI.Activate();
                    connectingUI.Deactivate();
                    connectFailedUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = searchUI;
                    _networkManager.ConnectAsync().Forget();
                    break;
                case PlaySettingState.Connecting:
                    playModeTypeUI.Deactivate();
                    searchUI.Deactivate();
                    connectingUI.Activate();
                    connectFailedUI.Deactivate();
                    confirmUI.Deactivate();
                    _activeUI = connectingUI;
                    break;
                case PlaySettingState.Failed:
                    playModeTypeUI.Deactivate();
                    searchUI.Deactivate();
                    connectingUI.Deactivate();
                    connectFailedUI.Activate();
                    confirmUI.Deactivate();
                    _activeUI = connectFailedUI;
                    break;
                case PlaySettingState.Confirm:
                    playModeTypeUI.Deactivate();
                    searchUI.Deactivate();
                    connectingUI.Deactivate();
                    connectFailedUI.Deactivate();
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