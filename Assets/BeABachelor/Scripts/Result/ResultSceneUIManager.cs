using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultSceneUIManager : MonoBehaviour
    {
        [SerializeField] private Text resultText;
        [SerializeField] private Text scoreText;
        
        [Inject] private IResultManager _resultManager;

        private void Start()
        {
            resultText.text = _resultManager.ResultText;
            scoreText.text = _resultManager.ScoreText;
        }
    }
}