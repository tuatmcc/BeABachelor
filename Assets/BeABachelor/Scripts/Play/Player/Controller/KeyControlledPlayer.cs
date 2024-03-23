using BeABachelor.Play.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace BeABachelor.Play.Player
{
    public class KeyControlledPlayer : MonoBehaviour, IPlayable, IItemCollectable
    {
        public event Action<long> OnStaminaChanged;

        [Inject] GameManager _gameManager;

        [SerializeField] private DefaultPlaySceneParams.Direction directionX = DefaultPlaySceneParams.Direction.PLUS;
        [SerializeField] private DefaultPlaySceneParams.Direction directionY = DefaultPlaySceneParams.Direction.PLUS;

        public long Stamina
        {
            get => stamina;
            set
            {
                stamina = value;
                OnStaminaChanged?.Invoke(stamina);
            }
        }

        public bool CantRun { get; private set; }

        private InputAction move, run;
        private CancellationTokenSource _cts;
        private long stamina;
        private bool playing = false;
        private bool finished = false;

        private void Awake()
        {
            _gameManager.OnGameStateChanged += OnGameStarted;
        }

        private void Start()
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                move = playerInput.actions["Move"];
                run = playerInput.actions["Run"];
            }


            _cts = new();
            ManageStaminaAsync(_cts.Token).Forget();
        }

        private void Update()
        {
            if(playing)
            {
                // 入力から移動量を決定
                var inputMoveAxis = move.ReadValue<Vector2>();
                var movedir = new Vector3(inputMoveAxis.x * (float)directionX, 0.0f, inputMoveAxis.y * (float)directionY);
                movedir *= Time.deltaTime * DefaultPlaySceneParams.DefaultSpeed * 
                    (CantRun ? DefaultPlaySceneParams.NoStaminaSpeed : (run.IsPressed() ? DefaultPlaySceneParams.RunningSpeed : 1.0f));
                transform.position += movedir;
                // 移動方向への回転
                transform.forward = Vector3.Slerp(transform.forward, movedir, Time.deltaTime * DefaultPlaySceneParams.RotateSpeed);
            }
        }

        private async UniTask ManageStaminaAsync(CancellationToken token)
        {
            Stamina = DefaultPlaySceneParams.StaminaMax;
            CantRun = false;
            while(!token.IsCancellationRequested || (!finished && playing))
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken : token);
                if (run.IsPressed())
                {
                    Stamina -= 1;
                    if(Stamina == 0)
                    {
                        CantRun = true;
                        while(Stamina != DefaultPlaySceneParams.StaminaMax)
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken : token);
                            Stamina += 1;
                        }
                        CantRun = false;
                    }
                }
                else
                {
                    Stamina = Math.Min(Stamina + 1, DefaultPlaySceneParams.StaminaMax);
                }
            }
        }

        private void OnGameStarted(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    playing = true;
                    break;
                case GameState.Finished:
                    playing = false;
                    finished = true;
                    break;
                default:
                    break;
            }
        }
    }
}
