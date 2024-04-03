using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultSceneUIManager : MonoBehaviour
    {
        [SerializeField] private Text resultText;
        [SerializeField] private Text scoreText;
        [SerializeField] private PlayerInput playerInput;
        
        [Inject] private IResultManager _resultManager;

        private void Start()
        {
            resultText.text = _resultManager.ResultText;
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