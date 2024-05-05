using System;
using BeABachelor.Interface;
using BeABachelor.Play.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play.UI
{
    public class StaminaPresenter : MonoBehaviour
    {
        [SerializeField] private Color green;
        [SerializeField] private Color yellow;
        [SerializeField] private Color red;
        
        [Inject] IGameManager gameManager;
        [Inject] PlaySceneManager playSceneManager;

        private KeyControlledPlayer player;
        private Image image;

        void Start()
        {
            image = GetComponent<Image>();
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
            image.color = player.CantRun ? red : image.fillAmount > 0.5 ? green : image.fillAmount > 0.3 ? yellow : red;
            image.enabled = !player.IsStaminaFull;
        }

        private void EnableImage(GameState gameState)
        {
            // if (gameState == GameState.Playing) image.enabled = true;
        }
    }
}