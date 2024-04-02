using System;
using BeABachelor.Interface;
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

        public Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
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

        // public void SelectPlayMode(InputAction.CallbackContext context)
        // {
        //     if (_state != PlaySettingState.PlayMode) { return; }
        //
        //     switch (context.action.name)
        //     {
        //         case "Left":
        //             _gameManager.PlayType = PlayType.Solo;
        //             break;
        //         case "Right":
        //             _gameManager.PlayType = PlayType.Multi;
        //             break;
        //         default:
        //             return;
        //     }
        //
        //     if (_state == PlaySettingState.PlayMode) _state++;
        // }
        //
        // public void SelectPlayerType(InputAction.CallbackContext context)
        // {
        //     if (_state != PlaySettingState.PlayerType) { return; }
        //
        //     switch (context.action.name)
        //     {
        //         case "Left":
        //             _gameManager.PlayerType = PlayerType.Hakken;
        //             hakken.SetActive(true);
        //             break;
        //         case "Right":
        //             _gameManager.PlayerType = PlayerType.Kouken;
        //             kouken.SetActive(true);
        //             break;
        //         default:
        //             return;
        //     }
        //
        //     if (_state == PlaySettingState.PlayerType) _state++;
        // }
        //
        // public void ConfirmSetting(InputAction.CallbackContext context)
        // {
        //     if (_state != PlaySettingState.Confirm) { return; }
        //
        //     if (context.action.name == "Left")
        //     {
        //         // mState = PlaySettingState.PlayMode;
        //         // mGameManager++;
        //         Debug.Log("Confirmed");
        //     }
        //     else if (context.action.name == "Right")
        //     {
        //         _state = PlaySettingState.PlayMode;
        //     }
        // }

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
                    break;
            }
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
            }
        }

        private void Start()
        {
            OnPlaySettingStateChanged += state =>
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
            };
            State = PlaySettingState.PlayMode;
        }
    }
}