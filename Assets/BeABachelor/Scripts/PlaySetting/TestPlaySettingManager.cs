using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor
{
    [RequireComponent(typeof(PlayerInput))]
    public class TestPlaySettingManager : MonoBehaviour, IPlaySettingManager
    {
        // [Inject] private GameManager mGameManager;
        private PlaySettingState mState;
        private PlayerInput mPlayerInput;

        public Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        public PlaySettingState State
        {
            get => mState;
            set
            {
                mState = value;
                OnPlaySettingStateChanged?.Invoke(mState);
            }
        }
        public GameObject mModeSelector;
        public GameObject mTypeSelector;
        public GameObject mConfirmSelector;
        public GameObject mHakken;
        public GameObject mKouken;

        public void Awake()
        {
            mPlayerInput = GetComponent<PlayerInput>();
        }

        public void OnEnable()
        {
            mPlayerInput.actions["Left"].performed += OnSelect;
            mPlayerInput.actions["Right"].performed += OnSelect;
        }

        public void OnDisable()
        {
            mPlayerInput.actions["Left"].performed -= OnSelect;
            mPlayerInput.actions["Right"].performed -= OnSelect;
        }

        public void SelectPlayMode(InputAction.CallbackContext context)
        {
            if (mState != PlaySettingState.PlayMode) { return; }

            if (context.action.name == "Left")
            {
                //mGameManager.PlayType = PlayType.Solo;
            }
            else if (context.action.name == "Right")
            {
                //mGameManager.PlayType = PlayType.Multi;
            }
            else
            {
                //mGameManager.PlayType = PlayType.NotSelected;
                return;
            }

            if (mState == PlaySettingState.PlayMode) mState++;
        }

        public void SelectPlayerType(InputAction.CallbackContext context)
        {
            if (mState != PlaySettingState.PlayerType) { return; }

            if (context.action.name == "Left")
            {
                //mGameManager.PlayerType = PlayerType.Hakken;
                mHakken.SetActive(true);
            }
            else if (context.action.name == "Right")
            {
                //mGameManager.PlayerType = PlayerType.Kouken;
                mKouken.SetActive(true);
            }
            else
            {
                //mGameManager.PlayerType = PlayerType.NotSelected;
                return;
            }

            if (mState == PlaySettingState.PlayerType) mState++;
        }

        public void ConfirmSetting(InputAction.CallbackContext context)
        {
            if (mState != PlaySettingState.Confirm) { return; }

            if (context.action.name == "Left")
            {
                // mState = PlaySettingState.PlayMode;
                // mGameManager++;
                Debug.Log("Confirm");
            }
            else if (context.action.name == "Right")
            {
                mState = PlaySettingState.PlayMode;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            switch (mState)
            {
                case PlaySettingState.PlayMode:
                    SelectPlayMode(context);
                    break;
                case PlaySettingState.PlayerType:
                    SelectPlayerType(context);
                    break;
                case PlaySettingState.Confirm:
                    ConfirmSetting(context);
                    break;
            }
        }

        void Start()
        {
            mState = PlaySettingState.PlayMode;
            mModeSelector.SetActive(true);
            mTypeSelector.SetActive(false);
            mConfirmSelector.SetActive(false);
            mHakken.SetActive(false);
            mKouken.SetActive(false);
        }

        void Update()
        {
            switch (mState)
            {
                case PlaySettingState.PlayMode:
                    mModeSelector.SetActive(true);
                    mTypeSelector.SetActive(false);
                    mConfirmSelector.SetActive(false);
                    mHakken.SetActive(false);
                    mKouken.SetActive(false);
                    break;
                case PlaySettingState.PlayerType:
                    mModeSelector.SetActive(false);
                    mTypeSelector.SetActive(true);
                    mConfirmSelector.SetActive(false);
                    mHakken.SetActive(false);
                    mKouken.SetActive(false);
                    break;
                case PlaySettingState.Confirm:
                    mModeSelector.SetActive(false);
                    mTypeSelector.SetActive(false);
                    mConfirmSelector.SetActive(true);
                    break;
            }
            Debug.Log(mState);
        }
    }
}
