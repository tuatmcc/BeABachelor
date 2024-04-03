using BeABachelor.Interface;
using BeABachelor.Play.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class StaminaBackgroundPresenter : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] Sprite green;
        [SerializeField] Sprite red;

        [Inject] IGameManager gameManager;
        [Inject] PlaySceneManager playSceneManager;

        private KeyControlledPlayer player;

        void Start()
        {
            image.enabled = false;
            player = playSceneManager.GetKeyControlledPlayer();
            gameManager.OnGameStateChanged += EnableImage;
            player.OnRunnableChanged += OnRunnableChanged;
        }

        private void OnDestroy()
        {
            gameManager.OnGameStateChanged -= EnableImage;
            player.OnRunnableChanged -= OnRunnableChanged;
        }

        private void OnRunnableChanged(bool runnable)
        {
            image.sprite = runnable ? green : red;
        }

        private void EnableImage(GameState gameState)
        {
            if(gameState == GameState.Playing) image.enabled = true;
        }
    }
}
