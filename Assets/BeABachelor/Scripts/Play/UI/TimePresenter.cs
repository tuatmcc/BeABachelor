using BeABachelor.Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class TimePresenter : MonoBehaviour
    {
        [SerializeField] Text text;

        [Inject] IGameManager gameManager;
        [Inject] PlaySceneManager playSceneManager;

        void Start()
        {
            text.enabled = false;
            playSceneManager.OnTimeChanged += OnTimeChanged;
            gameManager.OnGameStateChanged += EnableText;
        }

        private void OnDestroy()
        {
            gameManager.OnGameStateChanged -= EnableText;
            playSceneManager.OnTimeChanged -= OnTimeChanged;
        }

        private void OnTimeChanged(int time)
        {
            text.text = $"{time / 60 : 00}:{time % 60 : 00}";
        }

        private void EnableText(GameState gameState)
        {
            if(gameState == GameState.Playing)  text.enabled = true;
        }
    }
}
