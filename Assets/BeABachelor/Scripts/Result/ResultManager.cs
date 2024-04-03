using System;
using BeABachelor.Interface;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultManager : IResultManager
    {
        [Inject] private IGameManager _gameManager;

        public string ResultText => _gameManager.ResultState switch
        {
            ResultState.Win => "卒業成功！",
            ResultState.Lose => "留年...",
            ResultState.Draw => "仲良く卒業！",
            _ => throw new NotImplementedException()
        };

        public string ScoreText => _gameManager.Score.ToString();

        public void ToTile()
        {
            _gameManager.GameState = GameState.Title;
        }
    }
}