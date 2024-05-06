using BeABachelor.Interface;
using BeABachelor.Play.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class StaminaBackgroundPresenter : MonoBehaviour
    {
        [Inject] IGameManager gameManager;
        [Inject] PlaySceneManager playSceneManager;

        private KeyControlledPlayer player;
        private Image image;

        void Start()
        {
            gameObject.SetActive(false);
            image = GetComponent<Image>();
            // image.enabled = false;
            player = playSceneManager.GetKeyControlledPlayer();
            gameManager.OnGameStateChanged += EnableImage;
            player.OnStaminaChanged += OnStaminaChanged;
        }

        private void OnDestroy()
        {
            gameManager.OnGameStateChanged -= EnableImage;
            player.OnStaminaChanged -= OnStaminaChanged;    
        }
        
        private void OnStaminaChanged(long stamina)
        {
            image.enabled = !player.IsStaminaFull;
        }

        private void EnableImage(GameState gameState)
        {
            if(gameState == GameState.Playing) gameObject.SetActive(true);
        }
    }
}
