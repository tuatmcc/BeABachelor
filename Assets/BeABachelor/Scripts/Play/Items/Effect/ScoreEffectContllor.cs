using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items.Effect
{
    public class ScoreEffectContllor : MonoBehaviour
    {
        [SerializeField] private GameObject scoreEffect;
        [SerializeField] private GameObject endEffect;
        
        [Inject] private IGameManager _gameManager;

        private CancellationTokenSource _cts;
        private CancellationToken _ct;
        
        private void Start()
        {
            _gameManager.OnScoreChanged += OnScoreChanged;
            _cts = new CancellationTokenSource();
            _ct = _cts.Token;
        }
        
        private void OnDestroy()
        {
            _gameManager.OnScoreChanged -= OnScoreChanged;
            _cts.Cancel();
        }
        
        private void OnScoreChanged(int _)
        {
            UniTask.Create(async () =>
            {
                var effect = Instantiate(scoreEffect, transform);
                await UniTask.Delay(500, cancellationToken: _ct);
                var end = Instantiate(endEffect, new Vector3(-96.5f, 46, 0), Quaternion.identity, transform);
                await UniTask.Delay(10000, cancellationToken: _ct);
                Destroy(effect);
                Destroy(end);
            }).Forget();
        }
    }
}