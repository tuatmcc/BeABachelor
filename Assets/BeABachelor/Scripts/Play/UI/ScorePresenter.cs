using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class ScorePresenter : MonoBehaviour
    {
        [SerializeField] Text text;

        [Inject] IGameManager gameManager;

        private CancellationTokenSource _cts;

        void Start()
        {
            text.text = $"00/30単位";
            text.enabled = false;
            gameManager.OnScoreChanged += OnScoreChanged;
            gameManager.OnGameStateChanged += EnaleText;
            _cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            gameManager.OnScoreChanged -= OnScoreChanged;
            gameManager.OnGameStateChanged -= EnaleText;
            _cts.Cancel();
        }

        private void OnScoreChanged(int score)
        {
            Debug.Log($"Score : {score}");
            UniTask.Create(async () =>
            {
                await UniTask.Delay(500, cancellationToken: _cts.Token);
                text.text = $"{score :00}/30単位";
                return UniTask.CompletedTask;
            }).Forget();
        }

        private void EnaleText(GameState gameState)
        {
            if(gameState == GameState.Playing) text.enabled = true;
        }
    }
}
