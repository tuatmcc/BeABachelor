using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeABachelor
{
    public interface ITitleManager
    {
        void Awake();
        void OnEnable();
        void OnDisable();
        void OnKeyDown(InputAction.CallbackContext context);
    }
}
