using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items.Effect
{
    public class ScoreEffectContllor : MonoBehaviour
    {
        [SerializeField] private GameObject scoreEffect;
        [SerializeField] private GameObject endEffect;
        
        [Inject] private IGameManager _gameManager;
        
        private void Start()
        {
            _gameManager.OnScoreChanged += OnScoreChanged;
        }
        
        private void OnDestroy()
        {
            _gameManager.OnScoreChanged -= OnScoreChanged;
        }
        
        private void OnScoreChanged(int _)
        {
            UniTask.Create(async () =>
            {
                var effect = Instantiate(scoreEffect, transform);
                await UniTask.Delay(500);
                var end = Instantiate(endEffect, new Vector3(-96.5f, 46, 0), Quaternion.identity, transform);
                await UniTask.Delay(10000);
                Destroy(effect);
                Destroy(end);
            }).Forget();
        }
    }
}