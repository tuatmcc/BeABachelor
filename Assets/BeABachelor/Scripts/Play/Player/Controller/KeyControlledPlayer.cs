using BeABachelor.Play.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BeABachelor.Play.Player
{
    public class KeyControlledPlayer : MonoBehaviour, IPlayable, IItemCollectable
    {
        [Inject] GameManager _gameManager;

        [SerializeField] private DefaultPlaySceneParams.Direction directionX = DefaultPlaySceneParams.Direction.PLUS;
        [SerializeField] private DefaultPlaySceneParams.Direction directionY = DefaultPlaySceneParams.Direction.PLUS;

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
                // 入力から移動量を決定
                var inputMoveAxis = move.ReadValue<Vector2>();
                var movedir = new Vector3(inputMoveAxis.x * (float)directionX, 0.0f, inputMoveAxis.y * (float)directionY);
                movedir *= Time.deltaTime * DefaultPlaySceneParams.DefaultSpeed * (run.IsPressed() ? DefaultPlaySceneParams.RunningSpeed : 1.0f);
                transform.position += movedir;
                // 移動方向への回転
                transform.forward = Vector3.Slerp(transform.forward, movedir, Time.deltaTime * DefaultPlaySceneParams.RotateSpeed);
            }
        }
    }
}
