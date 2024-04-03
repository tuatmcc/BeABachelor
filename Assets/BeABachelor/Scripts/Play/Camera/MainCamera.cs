using BeABachelor.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Camera
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        [Inject] IGameManager _gameManager;
        [Inject] PlaySceneManager _playSceneManager;

        private float flag = 1;

        private void Awake()
        {
            _gameManager.OnGameStateChanged += SetTarget;
        }

        void Start()
        {
        }

        void Update()
        {
                transform.position = target.transform.position - new Vector3(0.0f, -7.0f, 15.0f * flag);
        }

        private void OnDestroy()
        {
            _gameManager.OnGameStateChanged -= SetTarget;
        }

        private void SetTarget(GameState gameState)
        {
            if(gameState == GameState.CountDown)
            {
                target = _playSceneManager.GetPlayerObject();

                // とりあえず名前解決
                if(target.name == "HakkenPlayer")
                {
                    flag = 1.0f;
                }
                else
                {
                    flag = -1.0f;
                    transform.localEulerAngles += new Vector3(0, 180, 0);
                }
            }
        }
    }
}
