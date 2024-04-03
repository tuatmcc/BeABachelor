using BeABachelor.Interface;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultManager : IResultManager
    {
        [Inject] private IGameManager _gameManager;
        
        public string ResultText {
            get;
        }
        public string ScoreText => _gameManager.Score.ToString();
    }
}