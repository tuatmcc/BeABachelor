using BeABachelor.Interface;
using BeABachelor.Play.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class StaminaPresenter : MonoBehaviour
    {
        [SerializeField] Image image;

        [Inject] IGameManager gameManager;
        [Inject] PlaySceneManager playSceneManager;

        private KeyControlledPlayer player;

        void Start()
        {
            image.enabled = false;
            player = playSceneManager.GetKeyControlledPlayer();
            player.OnStaminaChanged += OnStaminaChanged;
            gameManager.OnGameStateChanged += EnableImage;
        }

        private void OnDestroy()
        {
            gameManager.OnGameStateChanged -= EnableImage;
            player.OnStaminaChanged -= OnStaminaChanged;
        }

        private void OnStaminaChanged(long stamina)
        {
            image.fillAmount = (float)stamina / DefaultPlaySceneParams.StaminaMax;
        }

        private void EnableImage(GameState gameState)
        {
            if (gameState == GameState.Playing) image.enabled = true;
        }
    }
}