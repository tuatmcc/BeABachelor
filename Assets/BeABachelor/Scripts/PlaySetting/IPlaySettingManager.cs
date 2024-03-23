using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeABachelor
{
    public interface IPlaySettingManager
    {
        public Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        public PlaySettingState State { get; set; }
        public void Awake();
        public void OnEnable();
        public void OnDisable();
        void SelectPlayMode(InputAction.CallbackContext context);
        void SelectPlayerType(InputAction.CallbackContext context);
        void ConfirmSetting(InputAction.CallbackContext context);
        void OnSelect(InputAction.CallbackContext context);
    }
}
