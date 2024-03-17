using BeABachelor.Play.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.Play.Player
{
    public class KeyControlledPlayer : MonoBehaviour, IPlayable, IItemCollectable
    {
        [Inject] GameManager _gameManager;

        InputAction move, run;

        private void Start()
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                move = playerInput.actions["Move"];
                run = playerInput.actions["Run"];
            }
        }

        private void Update()
        {
            if(_gameManager.GameState == GameState.Playing)
            {
                var inputMoveAxis = move.ReadValue<Vector2>();
                transform.position += 
                    (transform.forward * inputMoveAxis.y + transform.right * inputMoveAxis.x) * Time.deltaTime 
                    * DefaultPlaySceneParams.DefaultSpeed * (run.IsPressed() ? DefaultPlaySceneParams.RunningSpeed : 1.0f);
            }
        }
    }
}
