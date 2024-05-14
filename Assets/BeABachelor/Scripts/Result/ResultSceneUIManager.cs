using System;
using BeABachelor.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultSceneUIManager : MonoBehaviour
    {
        // [SerializeField] private Image resultImage;
        [SerializeField] private Sprite gokokau;
        [SerializeField] private Sprite nakayokuGokaku;
        [SerializeField] private Sprite ryunen;
        [SerializeField] private Text scoreText;
        [SerializeField] private PlayerInput playerInput;
        
        [Inject] private IResultManager _resultManager;
        [Inject] private IGameManager _gameManager;

        private void Start()
        {
            // resultImage.sprite = _gameManager.ResultState switch
            // {
            //     ResultState.Win => gokokau,
            //     ResultState.Lose => ryunen,
            //     ResultState.Draw => nakayokuGokaku,
            //     _ => throw new ArgumentOutOfRangeException()
            // };
            scoreText.text = _resultManager.ScoreText;
            playerInput.actions["Space"].performed += ToTile;
        }
        
        private void OnDestroy()
        {
            playerInput.actions["Space"].performed -= ToTile;
        }

        private void ToTile(InputAction.CallbackContext context)
        {
            _resultManager.ToTile();
        }
    }
}