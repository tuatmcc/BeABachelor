using BeABachelor.Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class ScorePresenter : MonoBehaviour
    {
        [SerializeField] Text text;

        [Inject] IGameManager gameManager;

        void Start()
        {
            text.enabled = false;
            gameManager.OnScoreChanged += OnScoreChanged;
            gameManager.OnGameStateChanged += EnaleText;
        }

        private void OnDestroy()
        {
            gameManager.OnScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(int score)
        {
            Debug.Log($"Score : {score}");
            text.text = $"Score : {score : 000}";
        }

        private void EnaleText(GameState gameState)
        {
            if(gameState == GameState.Playing) text.enabled = true;
        }
    }
}
