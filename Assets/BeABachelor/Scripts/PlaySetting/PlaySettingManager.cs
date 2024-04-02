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
        [SerializeField] private GameObject playModeTypeUI;
        [SerializeField] private GameObject playerTypeUI;
        [SerializeField] private GameObject multiplaySettingUI;
        [SerializeField] private GameObject confirmUI;
        
        [SerializeField] private GameObject hakken;
        [SerializeField] private GameObject kouken;
        
        [Inject] private IGameManager _gameManager;
        private PlaySettingState _state;

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
        }

        public void OnDisable()
        {
            playerInput.actions["Left"].performed -= OnSelect;
            playerInput.actions["Right"].performed -= OnSelect;
        }

        public void SelectPlayMode(InputAction.CallbackContext context)
        {
            if (_state != PlaySettingState.PlayMode) { return; }

            switch (context.action.name)
            {
                case "Left":
                    _gameManager.PlayType = PlayType.Solo;
                    break;
                case "Right":
                    _gameManager.PlayType = PlayType.Multi;
                    break;
                default:
                    return;
            }

            if (_state == PlaySettingState.PlayMode) _state++;
        }

        public void SelectPlayerType(InputAction.CallbackContext context)
        {
            if (_state != PlaySettingState.PlayerType) { return; }

            switch (context.action.name)
            {
                case "Left":
                    _gameManager.PlayerType = PlayerType.Hakken;
                    hakken.SetActive(true);
                    break;
                case "Right":
                    _gameManager.PlayerType = PlayerType.Kouken;
                    kouken.SetActive(true);
                    break;
                default:
                    return;
            }

            if (_state == PlaySettingState.PlayerType) _state++;
        }

        public void ConfirmSetting(InputAction.CallbackContext context)
        {
            if (_state != PlaySettingState.Confirm) { return; }

            if (context.action.name == "Left")
            {
                // mState = PlaySettingState.PlayMode;
                // mGameManager++;
                Debug.Log("Confirmed");
            }
            else if (context.action.name == "Right")
            {
                _state = PlaySettingState.PlayMode;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            switch (_state)
            {
                case PlaySettingState.PlayMode:
                    SelectPlayMode(context);
                    break;
                case PlaySettingState.PlayerType:
                    SelectPlayerType(context);
                    break;
                case PlaySettingState.MultiplaySetting:
                    
                    break;
                case PlaySettingState.Confirm:
                    ConfirmSetting(context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void Start()
        {
            _state = PlaySettingState.PlayMode;
            OnPlaySettingStateChanged += _state =>
            {
                switch (_state)
                {
                    case PlaySettingState.PlayMode:
                        playModeTypeUI.SetActive(true);
                        break;
                    case PlaySettingState.PlayerType:
                        playModeTypeUI.SetActive(false);
                        playerTypeUI.SetActive(true);
                        break;
                    case PlaySettingState.MultiplaySetting:
                        playerTypeUI.SetActive(false);
                        multiplaySettingUI.SetActive(true);
                        break;
                    case PlaySettingState.Confirm:
                        multiplaySettingUI.SetActive(false);
                        playerTypeUI.SetActive(false);
                        confirmUI.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
                }
            };
        }
    }
}
