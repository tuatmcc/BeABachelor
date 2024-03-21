using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor
{
    [RequireComponent(typeof(PlayerInput))]
    public class TestTitleManager : MonoBehaviour, ITitleManager
    {
        [Inject] private GameManager mGameManager;
        private PlayerInput mPlayerInput;

        public void Awake()
        {
            mPlayerInput = GetComponent<PlayerInput>();
        }

        public void OnEnable()
        {
            if (mPlayerInput == null) { return; }
            mPlayerInput.onActionTriggered += OnKeyDown;
        }

        public void OnDisable()
        {
            if (mPlayerInput == null) { return; }
            mPlayerInput.onActionTriggered -= OnKeyDown;
        }

        public void OnKeyDown(InputAction.CallbackContext context)
        {
            if (context.action.name == "KeyDown")
            {
                // mGameManager++;
                Debug.Log("Imprement scene transition to GameScene.");
            }
        }
    }
}
