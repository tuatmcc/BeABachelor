using System;
using BeABachelor.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultPropsVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject successProps;
        [SerializeField] private GameObject failProps;

        [Inject] private IResultManager _resultManager;
        [Inject] private IGameManager _gameManager;

        private void Start()
        {
            // There is no Draw Animations for now... :(
            successProps.SetActive(_gameManager.ResultState != ResultState.Lose);
            failProps.SetActive(_gameManager.ResultState == ResultState.Lose);
        }
    }
}