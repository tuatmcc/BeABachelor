using BeABachelor.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.Title
{
    
    [RequireComponent(typeof(PlayerInput))]
    public class TitleManager : MonoBehaviour, ITitleManager
    {
        [Inject] private IGameManager _gameManager;
        private PlayerInput _playerInput;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        
        private void OnKeyDown(InputAction.CallbackContext context)
        {
            if (context.action.name == "Space")
            {
                _gameManager.GameState = GameState.Setting;
            }
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